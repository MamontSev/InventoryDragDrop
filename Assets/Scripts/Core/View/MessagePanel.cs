using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using DG.Tweening;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Inventory.Core.View
{
	public class MessagePanel:MonoBehaviour
	{
		[SerializeField]
		private GameObject InGo;

		[SerializeField]
		private GameObject FonPanel;

		[SerializeField]
		private TextMeshProUGUI MessageText;

		[SerializeField]
		private Button OkButton;

		[SerializeField]
		private Transform HidePos;

		[SerializeField]
		private Transform ShowPos;

		private void Start()
		{
			InGo.SetActive(false);
			FonPanel.SetActive(false);
		}

		public async UniTask Show(string message)				   
		{
			InGo.SetActive(true);
			FonPanel.SetActive(true);
			MessageText.text = message;
			OkButton.onClick.RemoveAllListeners();

			await InGo.transform
		   .DOMove(ShowPos.position , 0.5f)
		   .SetEase(Ease.OutBack)
		   .From(HidePos.position)
		   .ToUniTask(TweenCancelBehaviour.Kill , destroyCancellationToken);

			var tcs = new TaskCompletionSource<bool>();
			OkButton.onClick.AddListener(() => tcs.SetResult(true));
			await tcs.Task;

			await InGo.transform
			.DOMove(HidePos.position , 0.5f)
			.SetEase(Ease.OutBack)
			.From(ShowPos.position)
			.ToUniTask(TweenCancelBehaviour.Kill , destroyCancellationToken);

			InGo.SetActive(false);
			FonPanel.SetActive(false);

		}
	}
}
