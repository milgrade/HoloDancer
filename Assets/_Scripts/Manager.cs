using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Linq;
using UnityEngine.VR.WSA.Input;
using UnityEngine.Windows.Speech;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;

public class Manager : MonoBehaviour {

    public GameObject DancerPrefab;
    private Animator anim;
    private Boolean dancerExist = false;
    private GestureRecognizer gestureRecognizer;
    private KeywordRecognizer keywordRecognizer;

    delegate void KeywordAction(PhraseRecognizedEventArgs args);
    private Dictionary<string, KeywordAction> keywordCollection;
    private Dictionary<string, System.Action> keywords = new Dictionary<string, Action>();

    private GameObject scene;

    private UnityEngine.VR.WSA.SurfaceObserver.SurfaceDataReadyDelegate OnDataReady;


    private void Start()
    {
        gestureRecognizer = new GestureRecognizer();
        gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap);
        gestureRecognizer.TappedEvent += Recognizer_TappedEvent;
        gestureRecognizer.StartCapturingGestures();


        keywords.Add("Flip Verticle", () =>
        {
            //var focusedObject = GazeManager.Instance.FocusedObject;
            var focusedObject = GazeManager.Instance.HitObject;

            if (focusedObject != null)
                // call the OnSelect method of the focused object...
                focusedObject.SendMessage("OnSelect");
        });

        //keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        // everything else the same onward...


        keywordCollection = new Dictionary<string, KeywordAction>();

        keywordCollection.Add("Samba Dance", SambaDanceCommand);
        keywordCollection.Add("Salsa Dance", SalsaDanceCommand);
        keywordCollection.Add("YMCA Dance", YMCADanceCommand);
        keywordCollection.Add("Macarena Dance", MacarenaDanceCommand);
        keywordCollection.Add("Idle Dance", IdleDanceCommand);

        keywordRecognizer = new KeywordRecognizer(keywordCollection.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();

        // Register for the MakePlanesComplete event.
        //SurfaceMeshesToPlanes.Instance.MakePlanesComplete += SurfaceMeshesToPlanes_MakePlanesComplete;

        scene = GameObject.Find("ScenePlayerSpace");

          
        UnityEngine.VR.WSA.SurfaceObserver observer = new UnityEngine.VR.WSA.SurfaceObserver();

    }

    

    private void Update()
    {


        //int children = scene.transform.childCount;

        //for (int i = 0; i < children; i++)
        //{
        //    int layerMask = 1 << LayerMask.NameToLayer("SpatialSurface");


        //    GameObject surface = scene.transform.GetChild(i).gameObject;

        //    Debug.Log(surface.name);


        //}

        //Debug.Log(children.ToString());



    }

    private void SurfaceMeshesToPlanes_MakePlanesComplete(object source, System.EventArgs args)
    {
        //List<GameObject> horizontal = new List<GameObject>();

        //horizontal = SurfaceMeshesToPlanes.Instance.GetActivePlanes(PlaneTypes.Table | PlaneTypes.Floor);

        //if (horizontal.Count >= 1)
        //{
        //    Debug.Log("Boom!");
        //}
    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        KeywordAction keywordAction;

        if (keywordCollection.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke(args);
        }
    }

    private void Recognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        RaycastHit hitInfo;

        if (!dancerExist && Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, Mathf.Infinity))
        {          
            //front unknown...
            //Quaternion rotation = Camera.main.transform.localRotation;
            //rotation.x = 0;
            //rotation.z = 0;
            //transform.rotation = rotation;
                   
            //we know avatar is facing backwards so specify rotation manually...
            GameObject dancer = Instantiate(DancerPrefab, hitInfo.point, Quaternion.Euler(0, transform.eulerAngles.y + 180f, 0));
            anim = dancer.GetComponent<Animator>();
            dancerExist = true;

            Camera.main.gameObject.GetComponent<UnityEngine.VR.WSA.SpatialMappingRenderer>().enabled = false;
        }
    }

    private void MacarenaDanceCommand(PhraseRecognizedEventArgs args)
    {
        anim.Play("macarena_dance", -1, 0f);
    }

    private void YMCADanceCommand(PhraseRecognizedEventArgs args)
    {
        anim.Play("ymca_dance", -1, 0f);
    }

    private void SalsaDanceCommand(PhraseRecognizedEventArgs args)
    {
        anim.Play("salsa_dance", -1, 0f);
    }

    private void SambaDanceCommand(PhraseRecognizedEventArgs args)
    {
        anim.Play("samba_dance", -1, 0f);
    }

    private void IdleDanceCommand(PhraseRecognizedEventArgs args)
    {
        anim.Play("standing_idle", -1, 0f);
    }

    private void OnDestroy()
    {
        gestureRecognizer.TappedEvent -= Recognizer_TappedEvent;
        keywordRecognizer.OnPhraseRecognized -= KeywordRecognizer_OnPhraseRecognized;
    }

}