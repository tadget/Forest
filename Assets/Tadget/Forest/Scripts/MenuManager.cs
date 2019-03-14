namespace Tadget
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Tadget.Map;
    using TMPro;

    public class MenuManager : MonoBehaviour
    {
        private ChunkFactory chunks;
        private bool isLoadingTitle;

        public Animator panelAnimator;
        public Animator textAnimator;
        public TextMeshProUGUI title;
        public Color[] lerpColors;
        public TileObjects tileObjects;
        public MapSettings mapSettings;

        private void Start()
        {
            isLoadingTitle = true;
            title.color = lerpColors[0];
            tileObjects.Init();
            chunks = gameObject.AddComponent<ChunkFactory>().Init(mapSettings, tileObjects);
            chunks.Get(Vector3Int.zero, Chunk.ChunkType.BIOME, OnChunkCreated);
        }

        private void OnChunkCreated(Chunk chunk)
        {
            panelAnimator.SetTrigger("fadeIn");
            StartCoroutine(LerpTextAlpha());
        }

        private Color c;
        private IEnumerator LerpTextAlpha()
        {
            float t = 0f;
            while (t < 1f)
            {
                title.color = Color.Lerp(lerpColors[0], lerpColors[1], t);
                t += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            textAnimator.SetTrigger("pulse");
            isLoadingTitle = false;
        }

        private void Update()
        {
            if ((Input.touchCount > 0 || Input.GetMouseButtonDown(0)) && !isLoadingTitle)
            {
                SceneManager.LoadScene("main");
            }
        }
    }
}


