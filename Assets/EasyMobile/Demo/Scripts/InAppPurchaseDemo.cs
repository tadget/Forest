using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text;
using SgLib.UI;

#if EM_UIAP
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
#endif

namespace EasyMobile.Demo
{
    public class InAppPurchaseDemo : MonoBehaviour
    {
        public bool logProductLocalizedData;
        public GameObject curtain;
        public GameObject scrollableListPrefab;
        public GameObject isInitInfo;
        public Text ownedProductsInfo;
        public Text selectedProductInfo;
        public GameObject receiptViewer;
        public DemoUtils demoUtils;

        private IAPProduct selectedProduct;
        private List<IAPProduct> ownedProducts = new List<IAPProduct>();

        void OnEnable()
        {            
            IAPManager.PurchaseCompleted += IAPManager_PurchaseCompleted;
            IAPManager.PurchaseFailed += IAPManager_PurchaseFailed;
            IAPManager.RestoreCompleted += IAPManager_RestoreCompleted;
        }

        void OnDisable()
        {
            IAPManager.PurchaseCompleted -= IAPManager_PurchaseCompleted;
            IAPManager.PurchaseFailed -= IAPManager_PurchaseFailed;   
            IAPManager.RestoreCompleted -= IAPManager_RestoreCompleted;
        }

        void IAPManager_PurchaseCompleted(IAPProduct product)
        {
            if (!ownedProducts.Contains(product))
                ownedProducts.Add(product);
            
            MobileNativeUI.Alert("Purchased Completed", "The purchase of product " + product.Name + " has completed successfully. This is when you should grant the buyer digital goods.");
        }

        void IAPManager_PurchaseFailed(IAPProduct product)
        {
            MobileNativeUI.Alert("Purchased Failed", "The purchase of product " + product.Name + " has failed.");
        }

        void IAPManager_RestoreCompleted()
        {
            MobileNativeUI.Alert("Restore Completed", "Your purchases have been restored successfully.");
        }

        void Start()
        {
            receiptViewer.SetActive(false); 
            curtain.SetActive(!EM_Settings.IsIAPModuleEnable);

            if (logProductLocalizedData)
            {
                #if EM_UIAP
                foreach (IAPProduct p in EM_Settings.InAppPurchasing.Products)
                {
                    UnityEngine.Purchasing.ProductMetadata data = IAPManager.GetProductLocalizedData(p.Name);

                    if (data != null)
                    {
                        Debug.Log("Product Localized Title: " + data.localizedTitle);
                        Debug.Log("Localized Price: " + data.localizedPriceString);
                        Debug.Log("Product Localized Description: " + data.localizedDescription);
                    }
                    else
                    {
                        Debug.Log("Localized data is null");
                    }
                }
                #endif
            }

            StartCoroutine(CheckOwnedProducts());
        }

        void Update()
        {
            ownedProductsInfo.text = "All purchased products will be listed here.";

            // Check if IAP module is initialized.
            if (IAPManager.IsInitialized())
            {
                demoUtils.DisplayBool(isInitInfo, true, "isInitialized: TRUE");

                // Displayed own products
                StringBuilder strBuilder = new StringBuilder();
                bool moreThanOne = false;
                for (int i = 0; i < ownedProducts.Count; i++)
                {
                    var pd = ownedProducts[i];
                    if (!moreThanOne)
                        moreThanOne = true;
                    else
                        strBuilder.Append(", ");

                    strBuilder.Append(pd.Name);
                }

                var productStr = strBuilder.ToString();

                if (!string.IsNullOrEmpty(productStr))
                {
                    // Overwrite the text above.
                    ownedProductsInfo.text = productStr;
                }
            }
            else
            {
                demoUtils.DisplayBool(isInitInfo, false, "isInitialized: FALSE");
            }
        }

        public void SelectProduct()
        {
            var products = EM_Settings.InAppPurchasing.Products;

            if (products == null || products.Length == 0)
            {
                MobileNativeUI.Alert("Alert", "You don't have any IAP product. Please go to Window > Easy Mobile > Settings and add some.");
                selectedProduct = null;
                return;
            }

            var items = new Dictionary<string, string>();

            foreach (IAPProduct pd in products)
            {
                items.Add(pd.Name, pd.Type.ToString());
            }

            var scrollableList = ScrollableList.Create(scrollableListPrefab, "PRODUCTS", items);
            scrollableList.ItemSelected += OnItemSelected;
        }

        public void Purchase()
        {
            if (selectedProduct != null)
            {
                IAPManager.Purchase(selectedProduct.Name);

                // You can also do
                //IAPManager.Purchase(selectedProduct);

                // The advantage of method Purchase() that uses product name is you can use it with the constant product names
                // in the generated EM_IAPConstants class for compile-time error detecting.
            }
            else
            {
                MobileNativeUI.Alert("Alert", "Please select a product.");
            }
        }

        public void ViewReceipt()
        {
            if (selectedProduct == null)
            {
                MobileNativeUI.Alert("Alert", "Please select a product.");
                return;
            }
                
            #if EM_UIAP
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                AppleInAppPurchaseReceipt receipt = IAPManager.GetAppleIAPReceipt(selectedProduct.Name);

                if (receipt != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("- Product ID: " + receipt.productID);
                    sb.Append("\n- Original Purchase Date: " + receipt.originalPurchaseDate.ToShortDateString());
                    sb.Append("\n- Original Transaction ID: " + receipt.originalTransactionIdentifier);
                    sb.Append("\n- Purchase Date: " + receipt.purchaseDate.ToShortDateString());
                    sb.Append("\n- Transaction ID: " + receipt.transactionID);
                    sb.Append("\n- Quantity: " + receipt.quantity);
                    sb.Append("\n- Cancellation Date: " + receipt.cancellationDate.ToShortDateString());
                    sb.Append("\n- Subscription Expiration Date: " + receipt.subscriptionExpirationDate.ToShortDateString());

                    ShowReceiptViewer(sb.ToString());
                }
                else
                {
                    MobileNativeUI.Alert("Alert", "The receipt of this product could not be retrieved. Make sure the product is owned and receipt validation is enabled.");
                }
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                GooglePlayReceipt receipt = IAPManager.GetGooglePlayReceipt(selectedProduct.Name);

                if (receipt != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("- Package Name: " + receipt.packageName);
                    sb.Append("\n- Product ID: " + receipt.productID);
                    sb.Append("\n- Purchase Date: " + receipt.purchaseDate.ToShortDateString());
                    sb.Append("\n- Purchase State: " + receipt.purchaseState.ToString());
                    sb.Append("\n- Transaction ID: " + receipt.transactionID);
                    sb.Append("\n- Purchase Token: " + receipt.purchaseToken);

                    ShowReceiptViewer(sb.ToString());
                }
                else
                {
                    MobileNativeUI.Alert("Alert", "The receipt of this product could not be retrieved. Make sure the product is owned and receipt validation is enabled.");
                }
            }
            else
            {
                Debug.Log("Please test on an iOS or Android device.");
            }
            #else
            MobileNativeUI.Alert("Alert", "Please enable Unity IAP service.");
            #endif
        }

        public void ShowReceiptViewer(string receiptContent)
        {
            receiptViewer.transform.Find("Content/ReceiptText").GetComponent<Text>().text = receiptContent;
            receiptViewer.SetActive(true);
        }

        public void HideReceiptViewer()
        {
            receiptViewer.SetActive(false);
        }

        public void RestorePurchases()
        {
            IAPManager.RestorePurchases();
        }

        void OnItemSelected(ScrollableList list, string title, string subtitle)
        {
            list.ItemSelected -= OnItemSelected;
            selectedProduct = IAPManager.GetIAPProductByName(title);
            selectedProductInfo.text = "Selected product: " + selectedProduct.Name + " (" + selectedProduct.Type.ToString() + ")";
        }

        IEnumerator CheckOwnedProducts()
        {
            // Wait until the module is initialized
            if (!IAPManager.IsInitialized())
            {
                yield return new WaitForSeconds(0.5f);
            }

            // Display list of owned non-consumable products.
            var products = EM_Settings.InAppPurchasing.Products;
            if (products != null && products.Length > 0)
            {
                for (int i = 0; i < products.Length; i++)
                {
                    var pd = products[i];
                    if (IAPManager.IsProductOwned(pd.Name) && !ownedProducts.Contains(pd))
                    {
                        ownedProducts.Add(pd);
                    }
                }
            }
        }
    }
}

