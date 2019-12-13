using CustomExtensions;
using UnityEngine;
using MyBox;

namespace Generation
{
    [RequireComponent(typeof(MapGenerator))]
    [ExecuteInEditMode]
    public class GenerateGridInEditor : MonoBehaviour
    {
        MapGenerator generator;
        PromisedAction pAction = new PromisedAction();

        [Header("Custom Parameters")]
        [SerializeField, Tooltip("If you want the Grid to Automatically Generate on the below Parameters")]
        private bool autoGenerate = false;
        [SerializeField, ConditionalField("autoGenerate", false, true)]
        [Tooltip("Generate the Grid when the Theme is Changed")]
        private bool GenerateOnThemeChanged;
        [SerializeField, ConditionalField("autoGenerate", false, true)]
        [Tooltip("Generate the Grid when the Resolution is Changed")]
        private bool GenerateOnResolutionChanged;
        [SerializeField, ConditionalField("autoGenerate", false, true)]
        [Tooltip("Generate the Camera's Orthographic Size is Changed")]
        private bool GenerateOnOrthoSizeChanged;
        [SerializeField, ConditionalField("autoGenerate", false, true)]
        [Tooltip("Whether to adjust the Camera's Orthographic Size using the SetCameraOrthoSize parameter below this")]
        private bool AdjustCameraOrthoSize;
        [SerializeField, Range(1f, 20f), ConditionalField("AdjustCameraOrthoSize", false, true)]
        [Tooltip("This changes the Main Camera's Orthographic Size according to the Slider's value")]
        private int setCameraOrthoSize;

        [Header("General Information")]
        [SerializeField]
        [ReadOnly]
        private Vector2 lastResolution;
        [SerializeField]
        [ReadOnly]
        private Vector2 currentResolution;

        private GridAssetTheme.Theme lastGridTheme;
        private int lastOrthoSize;
        private void Awake()
        {
            generator = GetComponent<MapGenerator>();
            lastResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
            lastOrthoSize = (int)Camera.main.orthographicSize;
            ClearGrid();
            autoGenerate = false;
        }

        private void Update()
        {
            //don't run if we're not in editor mode!
            if (Application.isPlaying)
                return;

            if (!autoGenerate)
                return;

            if (AdjustCameraOrthoSize)
                Camera.main.orthographicSize = setCameraOrthoSize;

            if (GenerateOnOrthoSizeChanged)
                if (Camera.main.orthographicSize != lastOrthoSize)
                {
                    lastOrthoSize = (int)Camera.main.orthographicSize;
                    RunMapGeneratorInEditor();
                }

            currentResolution = new Vector2(Screen.width, Screen.height);

            if (GenerateOnThemeChanged)
            {
                //if you've changed the theme
                if (generator.MapTheme != lastGridTheme)
                {
                    RunMapGeneratorInEditor();
                }
            }

            if (GenerateOnResolutionChanged)
            {
                //if the resolution gets changed, we gotta re-generate to match!
                if (currentResolution != lastResolution)
                {
                    lastResolution = currentResolution;
                    RunMapGeneratorInEditor();
                }
            }
        }

        [ButtonMethod]
        private void RunMapGeneratorInEditor()
        {
            ClearGrid();
            //update the last theme to be the current selected one in the inspector window
            lastGridTheme = generator.MapTheme;
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

        [ButtonMethod]
        private void ClearGrid()
        {
            if (generator == null)
                return;

            while (generator.MapContainer.transform.childCount > 0)
            {
                DestroyImmediate(generator.MapContainer.transform.GetChild(0).gameObject);
            }
        }
    }
}
