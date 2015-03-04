using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//------------//
// Node Class //
//------------//

public class Node<T> {
	
	//------------//
	// Edge Class //
	//------------//
	
	protected class Edge<T> {
		public float _length;
		public float length {
			get {return this._length;}
			protected set {this._length = value;}
		}
		
		protected Node<T> _toNode;
		public Node<T> toNode {
			get {return this._toNode;}
			protected set {this._toNode = value;}
		}
		
		public Edge (float length, Node<T> toNode) {
			this.length = length;
			this.toNode = toNode;
		}
	}
	
	//----------//
	
	
	//-----------//
	// Variables //
	//-----------//

	public float distanceToSource;
	public Node<T> nodeToSource;

	protected List<Edge<T>> edges;
	
	protected T _value;
	public T value {
		get {return this._value;}
		protected set {this._value = value;}
	}

	//-------------//
	
	
	
	//------------------//
	// Public Functions //
	//------------------//
	
	public Node(T value) {
		this.value = value;
		this.edges = new List<Edge<T>>();
		this.distanceToSource = Mathf.Infinity;
		this.nodeToSource = null;
	}
	
	public void addNeighbor (Node<T> newNeighbor) {
		if (this.IsInEdges(newNeighbor)) {
			Debug.LogWarning ("Node with value " + newNeighbor.value + " already exists in graph");
		} else {
			Edge<T> newEdge = new Edge<T>(1, newNeighbor);
			this.edges.Add (newEdge);
		}
	}
	
	public void addNeightbor (Node<T> newNeighbor, float length) {
		if (this.IsInEdges(newNeighbor, length)) {
			Debug.LogWarning ("Node with value " + newNeighbor.value + " and length of " + length + " already exists in graph");
		} else {
			Edge<T> newEdge = new Edge<T>(length, newNeighbor);
			this.edges.Add (newEdge);
		}
	}
	
	
	// Functions that check if the edges of this node contains duplicates //
	
	public bool IsInEdges(Node<T> node, float length) {
		foreach (Edge<T> edge in edges) {
			if (edge.toNode.value.Equals(node.value) && edge.length == length) return true;
		}
		return false;
	}
	
	public bool IsInEdges(Node<T> node) {
		foreach (Edge<T> edge in edges) {
			if (edge.toNode.value.Equals(node.value)) return true;
		}
		return false;
	}
	
	public bool IsInEdges(T value, float length) {
		foreach (Edge<T> edge in edges) {
			if (edge.toNode.value.Equals(value) && edge.length == length) return true;
		}
		return false;
	}
	
	public bool IsInEdges(T value) {
		foreach (Edge<T> edge in edges) {
			if (edge.toNode.value.Equals(value)) return true;
		}
		return false;
	}
	
	//---------------------------//
}