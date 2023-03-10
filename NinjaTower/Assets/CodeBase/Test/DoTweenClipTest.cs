using System;
using Carotaa.Code;
using DG.Tweening;
using UnityEngine;

namespace Corataa.Code.Test
{
	public class DoTweenClipTest : MonoBehaviourAlt
	{
		[SerializeField] private DoTweenClip m_Clip;

		[SerializeField] private RectTransform rect;

		private Vector2 _startPos;

		[SerializeField] private CubicBezierCurve curveX;
		[SerializeField] private CubicBezierCurve curveY;
		[SerializeField] private Vector4 ease;
		[SerializeField] private AnimationCurve animCurve;

		private void Start()
		{
			_startPos = rect.anchoredPosition;
		}

		public void DebugTriggerPlayAbsolute()
		{
			transform.DoAnimationClipAbsolute(m_Clip);
		}

		public void DebugTriggerPlayRelativeEnd()
		{
			transform.DoAnimationClipRelative(m_Clip, m_Clip.Duration);
		}

		public void DebugTriggerPlayRelativeStart()
		{
			transform.DoAnimationClipRelative(m_Clip, 0f);
		}

		public void DebugTriggerDoAnchorCubic()
		{
			rect.anchoredPosition = _startPos;
			rect.DOAnchorPosBezier(curveX, curveY, 3f).SetEase(Ease.OutCubic);
		}

		public void DebugTriggerShowCurveDiff()
		{
			animCurve = curveX.Cast2AnimationCurve(ease);
			var easing = DOTweenBezier.BuildEasing(ease).GetEaseFunction();
			
			var diff = 0f;
			var normalized = 0f;
			for (var i = 0f; i <= 1f; i += 0.01f)
			{
				var t = (float) easing.Invoke(i);
				var x = curveX.Evaluate(t);
				var xp = animCurve.Evaluate(i);

				var aa = Mathf.Abs(x - xp);
				diff += aa;
				normalized += aa / Mathf.Max(x, 1f);
			}
			
			Debug.Log($"Difference: {diff * 0.01f} - {normalized * 0.01f}");
		}
	}
}