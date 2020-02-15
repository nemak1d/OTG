//-------------------------------------------
//
//  ビート
//
//-------------------------------------------
using UnityEngine;

namespace N1D.App
{
    public class Beat
    {
		//-----------------------------------
		// Method (public)
		//-----------------------------------
		public void Start(int targetTime)
		{
			TargetTime = targetTime;
			Progress = 0.0f;
			IsActive = true;
		}
		public void Update()
		{
			if (!IsActive)
			{
				return;
			}

			Progress = GetProgress();
		}

		public void Stop()
		{
			Progress = 1.0f;
			IsActive = false;
		}

		public float GetProgress(int delta = 0)
		{
			// TargetTime = タイミング時間の場合
			var endTime = CalculateActiveEndTime(TargetTime + delta);
			var startTime = endTime - CalculateActiveTime();

			return (float)(Metronome.instance.Time - startTime) / (float)(endTime - startTime);
		}

		public Evaluation Judge(int time)
		{
			Evaluation result;
			result.grade = JudgeGrade(time);
			result.timing = JudgeTiming(time);
			return result;
		}

		public EvaluationTiming JudgeTiming(int time)
		{
			if (time < TargetTime)
			{
				return EvaluationTiming.Early;
			}
			else if (time > TargetTime)
			{
				return EvaluationTiming.Late;
			}

			// TODO: マージンを設ける
			return EvaluationTiming.Just;
		}
		public EvaluationGrade JudgeGrade(int time)
		{
			var delta = Mathf.Abs(TargetTime - time);
			var settings = GameConfig.instance;
			
			if (delta <= settings.PerfectDelta)
			{
				return EvaluationGrade.Perfect;
			}
			else if (delta <= settings.GreatDelta)
			{
				return EvaluationGrade.Great;
			}
			else if (delta <= settings.GoodDelta)
			{
				return EvaluationGrade.Good;
			}

			return EvaluationGrade.Miss;
		}

		public bool IsValidRange(int delta = 0)
		{
			return IsValidRange(GetProgress(delta));
		}
		public bool IsValidRange(float progress)
		{
			// 0.0f, 1.0fを含めるかどうか悩むけどギリギリで判定取らないと思うのでいいや.
			return progress > 0.0f && progress < 1.0f;
		}
		public bool CalculateDrawPosition(int currentTimeDelta, Vector3 laneStartPosition, Vector3 laneEndPosition, out Vector3 position)
		{
			position = Vector3.zero;
			var progress = GetProgress(currentTimeDelta);

			if (!IsValidRange(progress))
			{
				return false;
			}

			position = Vector3.Lerp(laneStartPosition, laneEndPosition, progress);
			return true;
		}

		public void DrawTiming(Vector3 laneStartPosition, Vector3 laneEndPosition)
		{
			var width = 100.0f;
			var settings = GameConfig.instance;

			DrawTimingLine(-settings.PerfectDelta, laneStartPosition, laneEndPosition, width, Color.yellow);
			DrawTimingLine(settings.PerfectDelta, laneStartPosition, laneEndPosition, width, Color.yellow);
			DrawTimingLine(-settings.GreatDelta, laneStartPosition, laneEndPosition, width, Color.cyan);
			DrawTimingLine(settings.GreatDelta, laneStartPosition, laneEndPosition, width, Color.cyan);
			DrawTimingLine(-settings.GoodDelta, laneStartPosition, laneEndPosition, width, Color.blue);
			DrawTimingLine(settings.GoodDelta, laneStartPosition, laneEndPosition, width, Color.blue);
		}

		//-----------------------------------
		// Method (protected)
		//-----------------------------------

		protected int CalculateActiveTime()
		{
			return (int)(CalculateScaledTimingTime() / GameConfig.instance.TimingLineRate);
		}
		protected int CalculateActiveAfterTime()
		{
			return (int)(CalculateActiveTime() * (1.0f - GameConfig.instance.TimingLineRate));
		}
		protected int CalculateActiveEndTime(int targetTime)
		{
			return targetTime + CalculateActiveAfterTime();
		}
		protected int CalculateScaledTimingTime()
		{
			return GameConfig.instance.TimingTime * 60 / Metronome.instance.Bpm;
		}

		protected void DrawTimingLine(int currentTimeDelta, Vector3 laneStartPosition, Vector3 laneEndPosition, float width, Color color)
		{
			Vector3 point;
			if (!CalculateDrawPosition(currentTimeDelta, laneStartPosition, laneEndPosition, out point))
			{
				return;
			}
			float halfWidth = width * 0.5f;

			Debug.DrawLine(point + Vector3.left * halfWidth, point + Vector3.right * halfWidth, color);
		}
		//-----------------------------------
		// Property
		//-----------------------------------
		public bool IsActive { private set; get; } = false;
		public float Progress { private set; get; } = 0.0f;
		public int TargetTime { private set; get; } = 0;

		//-----------------------------------
		// Define
		//-----------------------------------

		//-----------------------------------
		// Field
		//-----------------------------------

		//-----------------------------------
		// Internal Class / Struct
		//-----------------------------------
	}
} // N1D.App