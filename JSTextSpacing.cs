using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JSTextSpacing : BaseMeshEffect
{
	public float textSpacing = 1.0f;

	public override void ModifyMesh(VertexHelper vh)
	{
		if (!IsActive() || 
			vh.currentVertCount == 0)
		{
			return;
		}

		Text text = GetComponent<Text>();
		if (text == null)
		{
			JSHelper.DebugLogError ("Missing Text component");
			return;
		}

		List<UIVertex> vertexs = new List<UIVertex>();
		vh.GetUIVertexStream (vertexs);
		int indexCount = vh.currentIndexCount;

		string[] lineTexts = text.text.Split('\n');

		Line[] lines = new Line[lineTexts.Length];

		for (int i = 0; i < lines.Length; i++)
		{
			if (i == 0)
			{
				lines[i] = new Line (0, lineTexts[i].Length + 1);
			}
			else if(i > 0 && i < lines.Length - 1)
			{
				lines[i] = new Line (lines[i - 1].EndVertexIndex + 1, lineTexts[i].Length + 1);
			}
			else
			{
				lines[i] = new Line (lines[i - 1].EndVertexIndex + 1, lineTexts[i].Length);
			}
		}

		UIVertex vt;

		for (int i = 0; i < lines.Length; i++)
		{
			for (int j = lines[i].StartVertexIndex + 6; j <= lines[i].EndVertexIndex; j++)
			{
				if (j < 0 || 
					j >= vertexs.Count)
				{
					continue;
				}
				vt = vertexs[j];
				vt.position += new Vector3(textSpacing * ((j - lines[i].StartVertexIndex) / 6), 0, 0);
				vertexs[j] = vt;

				if (j % 6 <= 2)
				{
					vh.SetUIVertex (vt, (j / 6) * 4 + j % 6);
				}
				if (j % 6 == 4)
				{
					vh.SetUIVertex (vt, (j / 6) * 4 + j % 6 - 1);
				}
			}
		}
	}
}

public class Line
{

	private int startVertexIndex = 0;

	public int StartVertexIndex
	{
		get
		{
			return startVertexIndex;
		}
	}

	private int endVertexIndex = 0;

	public int EndVertexIndex
	{
		get
		{
			return endVertexIndex;
		}
	}

	private int vertexCount = 0;

	public int VertexCount
	{
		get
		{
			return vertexCount;
		}
	}

	public Line(int index, int length)
	{
		startVertexIndex = index;
		endVertexIndex = length * 6 - 1 + startVertexIndex;
		vertexCount = length * 6;
	}
}

