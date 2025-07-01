using System.Collections.Generic;
using UnityEngine;
using Client = Supabase.Client;
using Controllers;
using UseCases;
using Repositories;
using Services;
using Entities;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ContentSelectionHandler : MonoBehaviour
{
    [SerializeField] private GameObject topicScrollView;
    [SerializeField] private GameObject catalogueScrollView;
    [SerializeField] private GameObject topicButtonPrefab;
    [SerializeField] private Transform topicContainer;
    [SerializeField] private GameObject catalogueButtonPrefab;
    [SerializeField] private GameObject addTopicButton;
    [SerializeField] private GameObject addCatalogueButton;
    [SerializeField] private TextMeshProUGUI addButtonText;
    [SerializeField] private Transform catalogueContainer;
    [SerializeField] private HexagonBackground bg = null;

    private static GameMode _gameMode;
    private static Client _supabase;
    public static List<CatalogueDTO> catalogues;
    public static List<Topic> topics;

    private ITopicRepository _topicRepo;
    private ICatalogueRepository _catalogueRepo;

    private FetchTopicsUseCase _topicsUseCase;
    private FetchCataloguesUseCase _cataloguesUseCase;
    private SupabaseRequestUseCase _supabaseRequestUseCase;

    private TopicController _topicController;
    private CatalogueController _catalogueController;


    // Start is called before the first frame update
    async void Start()
    {
        _supabase = SupabaseClientProvider.GetClient();

        _topicRepo = new SupabaseTopicRepository(_supabase);
        _topicsUseCase = new FetchTopicsUseCase(_topicRepo);
        _topicController = new TopicController(_topicsUseCase);

        _catalogueRepo = new SupabaseCatalogueRepository(_supabase);
        _cataloguesUseCase = new FetchCataloguesUseCase(_catalogueRepo);
        _supabaseRequestUseCase = new SupabaseRequestUseCase(_catalogueRepo);
        _catalogueController = new CatalogueController(_cataloguesUseCase, _supabaseRequestUseCase);

        _gameMode = Global.GetGameMode();

        try
        {
            topics = await _topicController.GetAllTopics();
            DisplayTopics(topics);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void DisplayTopics(List<Topic> topics)
    {
        if (_gameMode == GameMode.Statistics)
        {
            // to do
        }
        else if (_gameMode == GameMode.Editor && Global.EditorType == SceneLoader.EditorType.Topic)
        {
            addTopicButton.gameObject.SetActive(true);
        }

        foreach (var topic in topics)
        {
            string topicName = topic.name;

            if (_gameMode == GameMode.PracticeBook)
            {
                // to do: only show topics that have catalogues with questions marked for practive
            }

            GameObject topicButton = Instantiate(topicButtonPrefab, topicContainer);
            topicButton.GetComponentInChildren<TextMeshProUGUI>().text = topicName;

            topicButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                string topicName = topicButton.GetComponentInChildren<TextMeshProUGUI>().text;
                Topic topic = ContentSelectionHandler.topics.Find(t => t.name == topicName);

                Debug.Log("Topic name on Button: " + topic.name);

                if (_gameMode == GameMode.Editor && Global.EditorType == SceneLoader.EditorType.Topic)
                {
                    EditorManager.isNewTopic = false;
                    EditorManager.currentTopicName = topic.name;

                    Global.SetTmpTopic(topic);
                    Debug.Log(topic?.name);

                    SceneManager.LoadScene("Editor");
                    return;
                }

                Global.SetTmpTopic(topic);
                DisplayCatalogues(topicName);
            });
        }
    }

    public void CreateNewTopic()
    {
        if (_gameMode == GameMode.Editor && Global.EditorType == SceneLoader.EditorType.Topic)
        {
            EditorManager.isNewTopic = true;
            EditorManager.currentTopicName = null;
            Global.SetTmpTopic(null);

            SceneManager.LoadScene("Editor");
        }
    }

    public async void DisplayCatalogues(string topicName)
    {
        if (_gameMode == GameMode.Statistics)
        {
            // to do
        }
        else if (_gameMode == GameMode.Editor)
        {
            addCatalogueButton.gameObject.SetActive(true);
        }

        try
        {
            topicScrollView.SetActive(false);

            catalogues = await _catalogueController.GetCataloguesByTopic(topicName);

            foreach (var catalogue in catalogues)
            {
                if (_gameMode == GameMode.PracticeBook)
                {
                    // to do: only show catalogues that have questions marked for practice
                }

                Debug.Log("Topic name on display catalogues: " + catalogue.topicName);

                GameObject cButton = Instantiate(catalogueButtonPrefab, catalogueContainer);

                cButton.GetComponent<CatalogueButtonHandler>().SetBackground(bg);

                cButton.GetComponentInChildren<TextMeshProUGUI>().text = catalogue.name;
                
                // to do: set progress circle -> needs user catalogue data
            }

            catalogueScrollView.SetActive(true);
        }
        catch (Exception e)
        {
            catalogues = null;
            Debug.LogException(e);
            topicScrollView.SetActive(true);
        }
    }

    private void SetProgressCircle()
    {
        // to do
    }
}
