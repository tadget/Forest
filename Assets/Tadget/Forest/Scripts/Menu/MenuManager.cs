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
        private LoadingManager load;

        public Animator panelAnimator;
        public Animator textAnimator;
        public TextMeshProUGUI title;
        public Color[] lerpColors;
        public MapSettings mapSettings;

        private void Start()
        {
            load = GetComponent<LoadingManager>();
            GameData gameData = load.GetData(false);
            isLoadingTitle = true;
            title.color = lerpColors[0];
            chunks = gameObject.AddComponent<ChunkFactory>().Init(mapSettings);
            chunks.Get(Vector3Int.zero, gameData.isFirstTimePlaying ? Chunk.ChunkType.HOME : Chunk.ChunkType.BIOME, OnChunkCreated);
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


