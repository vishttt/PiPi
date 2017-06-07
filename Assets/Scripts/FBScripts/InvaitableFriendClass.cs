using System;
using System.Collections.Generic;


[Serializable]
public class Data
{
	public int height;

	public bool is_silhouette;

	public string url;

	public int width;
}


[Serializable]
public class Picture
{
	public Data data;
}


[Serializable]
public class Datum
{
	public string id;

	public string first_name;

	public Picture picture;
}


[Serializable]
public class Cursors
{
	public string before;

	public string after;
}


[Serializable]
public class Paging
{
	public Cursors cursors { get; set; }
}


[Serializable]
public class RootObject
{
	public List<Datum> data = new List<Datum> ();

	public Paging paging { get; set; }
}