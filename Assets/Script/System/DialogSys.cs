using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEditor;

public class DialogSys : MonoBehaviour {
    public static event Action<Story> OnCreateStory;
    
    [SerializeField] private TextAsset inkJSONAsset = null;
    public Story story;

    [SerializeField] private Text speakerName;
    [SerializeField] private Image playerFace;
    [SerializeField] private GameObject canvas = null;
    // UI Prefabs
    [SerializeField] private Text textPrefab = null;
    [SerializeField] private Button buttonPrefab = null;

    [SerializeField] private Image touchPanel = null;
    private Button chatting = null;
	
    [SerializeField] public List<string> chosen = new List<string>();
    [SerializeField] public List<int> chooseIndex = new List<int>();
    private string _eventName="";
    
    private const string PLAYER_NAME = "player_name";
    private const string PLAYER_FACE = "charTex_path";
    private const string BACK_PATH = "backTex_path";

    private bool isOver = false;
    void Awake () {
		// Remove the default message
		RemoveChildren();
		
		StartStory();
	}

    private void Start()
    {
	    MessageManager.Instance.Subscribe("CreateDiaButton_Close_Shop",CreateCloseButton);
    }

    private void OnEnable()
    {
	    RemoveChildren();
		
	    StartStory();
    }

    public void StartStory ()
    {
	    isOver = false;
		story = new Story (inkJSONAsset.text);
		chatting = touchPanel.GetComponent<Button>();
        if(OnCreateStory != null) OnCreateStory(story);
		RefreshView();
	}

	void RefreshView () {
		if (isOver)
		{
			return;
		}
		// Remove all the UI on screen
		RemoveChildren ();
		
		// Read all the content until we can't continue any more
		while (story.canContinue) {
			
			// Continue gets the next line of the story
			string text = story.Continue ();
			// This removes any white space from the text.
			//text = text.Trim();
			// Display the text on screen!
			text = text.Trim();
			if (text.Contains("Event."))
			{
				_eventName = text.Replace("Event.","");
				MessageManager.Instance.Dispatch(_eventName,null,true);
			}
			CreateContentView(text);
			
			//获取每次的文本变量内容，因为Ink文本产出的JSON文件只是文本，没法绑定对应变量
			//两个常量内部是字符串变量值，对应DialogBase类中的常量，两个是对应的
			
			string variableValue = story.variablesState[PLAYER_NAME].ToString();
			//speakerName.text = variableValue;
			text = $"{variableValue}:"+ text.Trim();
			
			string facePath = story.variablesState[PLAYER_FACE].ToString();
			//playerFace.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(facePath);
		}
		//判断结束条件是文本没有后续且没有任何选项（每段对话之间都有一个内容为"继续"的选项，并不是空，不然文本就直接跑到底了，如果怕选项内容和默认继续重复导致出错，麻烦加个标点符号
		if (!story.canContinue && story.currentChoices.Count == 0)
		{
			var buttons = canvas.GetComponentsInChildren<Button>();
			if (canvas.transform.childCount <= 0 || buttons.Length <=0)
			{
				RemoveChildren();
				this.transform.parent.gameObject.SetActive(false);
				return;
			}
		}
		
		//显示出所有的选择，且根据选择的内容绑定不同的事件，没选项单纯对话继续的就是会把事件绑定到UI中的touchpanel界面
		//想按钮触发自己改，建议和touchpanel的状态关联比较好
		if(story.currentChoices.Count > 0) {
			//这里就是touchPanel对话继续的面板，每次对话显示结束先恢复为true状态，之后再判断是否要关闭
			touchPanel.gameObject.SetActive(true);
			
			for (int i = 0; i < story.currentChoices.Count; i++)
			{
				Choice choice = story.currentChoices[i];

				if (choice.text.Contains("Event."))
				{
					_eventName = choice.text.Replace("Event.","");
					MessageManager.Instance.Dispatch(_eventName,null,true);
					continue;
				}
				// Tell the button what to do when we press it

				if (!choice.text.Trim().Equals("继续"))
				{
					//非默认选项情况下的处理
					touchPanel.gameObject.SetActive(false);
					Button button = CreateChoiceView(choice.text.Trim());
					button.onClick.AddListener(delegate { OnClickChoiceButton(choice); });
				}
				else
				{
					chatting.onClick.AddListener(delegate { OnContinueDia(choice); });
				}
			}
		}

	}

	void CreateCloseButton(object[] args)
	{
		Button button = CreateChoiceView("关闭商城");
		
		button.onClick.AddListener(delegate { MessageManager.Instance.Dispatch("Close_Shop"); });
		
	}

	// When we click the choice button, tell the story to choose that choice!

	void OnClickChoiceButton(Choice choice)
	{
		if (choice.index >= 0 && choice.index < story.currentChoices.Count)
		{
			story.ChooseChoiceIndex(choice.index);
			//这里就是得到选项的内容的情况
			chooseIndex.Add(choice.index);
			chosen.Add(choice.text);
			RefreshView();
		}
		else
		{
			Debug.LogError("Invalid choice index: " + choice.index);
		}
	}


	void OnContinueDia(Choice choice)
	{
		if (choice.index >= 0 && choice.index < story.currentChoices.Count)
		{
			story.ChooseChoiceIndex(choice.index);
			//这里使用单纯的移除目标监听的方法好像没有用，可能是索引问题反正没有其他功能，干脆全移除
			chatting.onClick.RemoveAllListeners();
			//为了防止挡住后面选项的触摸，触发完自己的功能就使他处于false的状态
			touchPanel.gameObject.SetActive(false);
			RefreshView();
			
		}
		else
		{
			Debug.LogError("Invalid choice index: " + choice.index);
		}
	}

	// Creates a textbox showing the the line of text
	void CreateContentView (string text) {
		
		
		Text storyText = Instantiate (textPrefab) as Text;
		storyText.text = text;
		storyText.transform.SetParent (canvas.transform, false);
		
		if (!_eventName.Equals(""))
		{
			isOver = true;
			MessageManager.Instance.PullMessageCache(_eventName);
			_eventName = "";
			Destroy(storyText);
		}
	}
	
	Button CreateChoiceView (string text) {

		Button choice = Instantiate (buttonPrefab) as Button;
		choice.transform.SetParent (canvas.transform, false);

		Text choiceText = choice.GetComponentInChildren<Text> ();
		choiceText.text = text;
		
		HorizontalLayoutGroup layoutGroup = choice.GetComponent <HorizontalLayoutGroup> ();
		layoutGroup.childForceExpandHeight = false;

		if (!_eventName.Equals(""))
		{
			isOver = true;
			MessageManager.Instance.PullMessageCache(_eventName);
			_eventName = "";
		}
		return choice;
	}
	
	void RemoveChildren () {
		int childCount = canvas.transform.childCount;
		for (int i = childCount - 1; i >= 0; --i) {
			GameObject.Destroy (canvas.transform.GetChild (i).gameObject);
		}
	}

}
