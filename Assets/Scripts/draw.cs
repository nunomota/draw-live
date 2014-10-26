using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class draw : MonoBehaviour {

	private Material mat;
	private Color lineColor = Color.black;

	private int thickness = 2;

	public List<Vector2> posList = new List<Vector2>();

	// Use this for initialization
	void Start () {
		mat = Resources.Load ("Line") as Material;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnPostRender() {
		GL.PushMatrix();
		mat.SetPass(0);
		GL.LoadPixelMatrix();
		GL.Color(lineColor);
		GL.Begin(GL.LINES);
		if (posList.Count > 0) {
			for (int i = 0; i < posList.Count-1; i++) {
				if (posList[i].x != -1 && posList[i+1].x != -1) {
					for (int j = -thickness; j <= thickness; j++) {
						//the lines below just add values to x and y. This adds a "cartoony" style
						//to whatever is drawn
						GL.Vertex3(posList[i].x + j, posList[i].y + j, 0);
						GL.Vertex3(posList[i+1].x + j, posList[i+1].y + j, 0);
					}
				}
			}
		}
		GL.End();
		GL.PopMatrix();
	}
}
