using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DogApiManager : MonoBehaviour
{
    [Header("UI - Breed List")]
    public Transform breedListContent;
    public GameObject breedEntryPrefab;

    [Header("UI - Pagination Controls")]
    public Button prevButton;
    public Button nextButton;
    public Button firstPageButton;
    public Button lastPageButton;
    public TMP_Text pageInfoText;
    public TMP_Text statusText;

    private const string ApiBase = "https://dogapi.dog/api/v2";
    private const int PageSize = 10;

    private int _currentPage = 1;
    private int _totalPages  = 1;
    private bool _isFetching = false;

    private void Start()
    {
        prevButton.onClick.AddListener(OnPrevPage);
        nextButton.onClick.AddListener(OnNextPage);
        firstPageButton.onClick.AddListener(OnFirstPage);
        lastPageButton.onClick.AddListener(OnLastPage);

        FetchPage(_currentPage);
    }

    private void OnPrevPage()
    {
        if (_isFetching || _currentPage <= 1) return;
        FetchPage(_currentPage - 1);
    }

    private void OnNextPage()
    {
        if (_isFetching || _currentPage >= _totalPages) return;
        FetchPage(_currentPage + 1);
    }

    private void OnFirstPage()
    {
        if (_isFetching || _currentPage == 1) return;
        FetchPage(1);
    }

    private void OnLastPage()
    {
        if (_isFetching || _currentPage == _totalPages) return;
        FetchPage(_totalPages);
    }

    private void FetchPage(int page)
    {
        if (_isFetching) return;
        StartCoroutine(FetchBreedsCoroutine(page));
    }

    private IEnumerator FetchBreedsCoroutine(int page)
    {
        _isFetching = true;
        SetButtonsInteractable(false);
        SetStatus($"Loading page {page}...");

        string url = $"{ApiBase}/breeds?page[number]={page}&page[size]={PageSize}";
        using UnityWebRequest req = UnityWebRequest.Get(url);
        req.SetRequestHeader("Accept", "application/json");

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            SetStatus($"Error: {req.error}");
        }
        else
        {
            try
            {
                BreedsResponse response = JsonConvert.DeserializeObject<BreedsResponse>(req.downloadHandler.text);
                _currentPage = response.meta.pagination.current;
                _totalPages  = response.meta.pagination.last;

                PopulateBreedList(response.data);
                UpdatePageInfo();
                SetStatus(string.Empty);
            }
            catch (Exception ex)
            {
                SetStatus($"Parse error: {ex.Message}");
                Debug.LogException(ex);
            }
        }

        _isFetching = false;
        SetButtonsInteractable(true);
        UpdateButtonStates();
    }

    private void PopulateBreedList(List<BreedData> breeds)
    {
        foreach (Transform child in breedListContent)
            Destroy(child.gameObject);

        if (breeds == null) return;

        foreach (var breed in breeds)
        {
            GameObject entry = Instantiate(breedEntryPrefab, breedListContent);
            var controller = entry.GetComponent<DogBreedEntry>();
            if (controller != null)
                controller.Populate(breed.attributes);
        }
    }

    private void UpdatePageInfo()
    {
        int pagesLeft = _totalPages - _currentPage;
        if (pageInfoText != null)
            pageInfoText.text = $"Page  {_currentPage}  /  {_totalPages}   |   {pagesLeft} page{(pagesLeft != 1 ? "s" : "")} left";
    }

    private void SetStatus(string msg)
    {
        if (statusText != null)
            statusText.text = msg;
    }

    private void SetButtonsInteractable(bool interactable)
    {
        prevButton.interactable      = interactable;
        nextButton.interactable      = interactable;
        firstPageButton.interactable = interactable;
        lastPageButton.interactable  = interactable;
    }

    private void UpdateButtonStates()
    {
        prevButton.interactable      = _currentPage > 1;
        nextButton.interactable      = _currentPage < _totalPages;
        firstPageButton.interactable = _currentPage > 1;
        lastPageButton.interactable  = _currentPage < _totalPages;
    }

    public class BreedsResponse
    {
        public List<BreedData> data { get; set; }
        public Meta meta { get; set; }
    }

    public class BreedData
    {
        public string id { get; set; }
        public BreedAttributes attributes { get; set; }
    }

    public class BreedAttributes
    {
        public string name { get; set; }
        public string description { get; set; }
        public bool hypoallergenic { get; set; }

        [JsonProperty("male_weight")]
        public WeightRange male_weight { get; set; }

        [JsonProperty("female_weight")]
        public WeightRange female_weight { get; set; }

        public LifeRange life { get; set; }
    }

    public class WeightRange
    {
        public float min { get; set; }
        public float max { get; set; }
    }

    public class LifeRange
    {
        public float min { get; set; }
        public float max { get; set; }
    }

    public class Meta
    {
        public Pagination pagination { get; set; }
    }

    public class Pagination
    {
        public int current { get; set; }
        public int next { get; set; }
        public int last { get; set; }
        public int records { get; set; }
    }
}
