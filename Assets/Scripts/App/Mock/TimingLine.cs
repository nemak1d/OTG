using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using N1D.Dbg;

public class TimingLine : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		m_StartTime = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {

		DrawLine(m_TimingPoint + (m_ToFromDirection * m_Length), Vector3.right, m_LineWidth, Color.yellow);
		DrawLine(m_TimingPoint, Vector3.right, m_LineWidth, Color.red);

		UpdateLine();
	}

	private void UpdateLine()
	{
		var delta = Time.realtimeSinceStartup - m_StartTime;
		var t = delta % m_LengthMoveTime;
		var point = Vector3.Lerp(m_TimingPoint + (m_ToFromDirection * m_Length), m_TimingPoint, t);
		DrawLine(point, Vector3.right, m_LineWidth, Color.green);
	}

	private void DrawLine(Vector3 point, Vector3 direction, float length, Color color)
	{
		var half = direction * length;
		var from = point - half;
		var to = point + half;

		Draw.instance.Line(from, to, color);
	}

	public Vector3 m_TimingPoint = Vector3.zero;
	public Vector3 m_ToFromDirection = Vector3.up;
	public float m_Length = 5.0f;

	public float m_LengthMoveTime = 1.0f;
	public float m_LineWidth = 3.0f;

	public float m_StartTime = 0.0f;
}
