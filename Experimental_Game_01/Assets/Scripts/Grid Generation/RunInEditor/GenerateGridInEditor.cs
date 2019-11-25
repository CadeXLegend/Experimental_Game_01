using CustomExtensions;
using UnityEngine;

namespace Generation
{
    [RequireComponent(typeof(GridGenerator))]
    [ExecuteInEditMode]
    public class GenerateGridInEditor : MonoBehaviour
    {
        GridGenerator generator;
        PromisedAction pAction = new PromisedAction();

        [Header("How To Use:")]
        [SerializeField]
        [TextArea(5, 10)]
        private string description = "To Generate a Grid, Change the Grid Theme on the Grid Generator Script. \n\nHover over any of the Properties to see their Description!";
        [SerializeField]
        [Tooltip("Let's start Generating Grids!")]
        private bool StartGenerating = false;

        [Header("Custom Parameters")]
        [SerializeField]
        [Tooltip("Generate the Grid when the Theme is Changed")]
        private bool GenerateOnThemeChanged;
        [SerializeField]
        [Tooltip("Generate the Grid when the Resolution is Changed")]
        private bool GenerateOnResolutionChanged;
        [SerializeField]
        [Tooltip("Generate the Camera's Orthographic Size is Changed")]
        private bool GenerateOnOrthoSizeChanged;
        [SerializeField]
        [Tooltip("Whether to adjust the Camera's Orthographic Size using the SetCameraOrthoSize parameter below this")]
        private bool AdjustCameraOrthoSize;
        [SerializeField]
        [Range(1f, 20f)]
        [Tooltip("This changes the Main Camera's Orthographic Size according to the Slider's value")]
        private int setCameraOrthoSize;

        [Header("General Information")]
        [SerializeField]
        [ReadOnly]
        private Vector2 lastResolution;
        [SerializeField]
        [ReadOnly]
        private Vector2 currentResolution;

        private GridAssetThemes.Theme lastGridTheme;
        private int lastOrthoSize;
        private void Awake()
        {
            generator = GetComponent<GridGenerator>();
            lastResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
            lastOrthoSize = (int)Camera.main.orthographicSize;
            CleanScene();
            StartGenerating = false;
        }

        private void Update()
        {
            //don't run if we're not in editor mode!
            if (Application.isPlaying)
                return;

            if (!StartGenerating)
                return;

            if (AdjustCameraOrthoSize)
                Camera.main.orthographicSize = setCameraOrthoSize;

            if (GenerateOnOrthoSizeChanged)
                if (Camera.main.orthographicSize != lastOrthoSize)
                {
                    lastOrthoSize = (int)Camera.main.orthographicSize;
                    RunMapGenerationInEditor();
                }

            currentResolution = new Vector2(Screen.width, Screen.height);

            if (GenerateOnThemeChanged)
            {
                //if you've changed the theme
                if (generator.GridTheme != lastGridTheme)
                {
                    RunMapGenerationInEditor();
                }
            }

            if (GenerateOnResolutionChanged)
            {
                //if the resolution gets changed, we gotta re-generate to match!
                if (currentResolution != lastResolution)
                {
                    lastResolution = currentResolution;
                    RunMapGenerationInEditor();
                }
            }
        }

        private void RunMapGenerationInEditor()
        {
            CleanScene();
            //update the last theme to be the current selected one in the inspector window
            lastGridTheme = generator.GridTheme;
            //let's assign some logging n all that
            pAction.ActionFailed += () =>
            {
                Debug.Log(pAction.ErrorMessage);
            };
            pAction.ActionSucceeded += () =>
            {
                Debug.Log("<color=blue><b>Grid successfully generated!</b></color>");
            };
            //and now, lastly, let's call the generation of the map!
            pAction.Call(() => { generator.Generate(); });
        }

        private void CleanScene()
        {
            while (generator.GridContainer.transform.childCount > 0)
            {
                DestroyImmediate(generator.GridContainer.transform.GetChild(0).gameObject);
            }
        }
    }
}
