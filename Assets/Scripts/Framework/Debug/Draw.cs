using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using N1D.Framework.Core;

namespace N1D.Framework.Dbg
{
	[DefaultExecutionOrder(-1)]
	public class Draw : Singleton<Draw>
	{
		private void Awake()
		{
			MightCreateMaterial();
		}

		private void Update()
		{
			m_Queue.Clear();
		}

		private void OnRenderObject()
		{
			m_Material.SetPass(0);
			GL.PushMatrix();

			foreach (var input in m_Queue)
			{
				
				GL.MultMatrix(transform.localToWorldMatrix);
				GL.Begin(GL.LINES);
				GL.Color(input.color);
				GL.Vertex3(input.from.x, input.from.y, input.from.z);
				GL.Vertex3(input.to.x, input.to.y, input.to.z);
				GL.End();
				
			}
			GL.PopMatrix();
		}

		public void Line(Vector3 from, Vector3 to)
		{
			Line(from, to, Color.red);
		}
		public void Line(Vector3 from, Vector3 to, Color color)
		{
			Input input;
			input.from = from;
			input.to = to;
			input.color = color;

			m_Queue.Enqueue(input);
		}

		private bool MightCreateMaterial()
		{
			if (m_Material == null)
			{
				var shader = Shader.Find("Hidden/Internal-Colored");
				m_Material = new Material(shader);
				m_Material.hideFlags = HideFlags.HideAndDontSave;
				m_Material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
				m_Material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				m_Material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
				m_Material.SetInt("_ZWrite", 0);

				return true;
			}
			return false;
		}

		[SerializeField]
		private Material m_Material = null;

		private struct Input
		{
			public Vector3 from;
			public Vector3 to;
			public Color color;
		}
		private Queue<Input> m_Queue = new Queue<Input>(100);
	}
} // N1D.Framework.Dbg