﻿/*
 * Taken from https://github.com/MadLittleMods/Radius/blob/master/Radius/Assets/Scripts/TextureTilingController.cs
 * via http://ericeastwood.com/blog/20/texture-tiling-based-on-object-sizescale-in-unity
 */

using UnityEngine;
using System.Collections;

public class TextureTilingController : MonoBehaviour {
	
	// Give us the texture so that we can scale proportianally the width according to the height variable below
	// We will grab it from the meshRenderer
	public Texture texture;
	public float textureToMeshZ = 1f; // Use this to contrain texture to a certain size
	public float xScale = 1;
	
	Vector3 prevScale = Vector3.one;
	float prevTextureToMeshZ = -1f;
	
	// Use this for initialization
	void Start () {
		this.prevScale = gameObject.transform.lossyScale;
		this.prevTextureToMeshZ = this.textureToMeshZ;
		this.texture = gameObject.GetComponent<Renderer>().sharedMaterial.mainTexture;
		
		this.UpdateTiling();
	}
	
	// Update is called once per frame
	void Update () {
		// If something has changed
		if(gameObject.transform.lossyScale != prevScale || !Mathf.Approximately(this.textureToMeshZ, prevTextureToMeshZ))
			this.UpdateTiling();
		
		// Maintain previous state variables
		this.prevScale = gameObject.transform.lossyScale;
		this.prevTextureToMeshZ = this.textureToMeshZ;
	}
	
	[ContextMenu("UpdateTiling")]
	void UpdateTiling()
	{
		// A Unity plane is 10 units x 10 units
		float planeSizeX = 1f;
		float planeSizeZ = 1f;
		
		// Figure out texture-to-mesh width based on user set texture-to-mesh height
		float textureToMeshX = ((float)this.texture.width/this.texture.height)*this.textureToMeshZ * xScale;
		
		gameObject.GetComponent<Renderer>().material.mainTextureScale = new Vector2(planeSizeX*gameObject.transform.lossyScale.x/textureToMeshX, planeSizeZ*gameObject.transform.lossyScale.z/textureToMeshZ);
	}
}