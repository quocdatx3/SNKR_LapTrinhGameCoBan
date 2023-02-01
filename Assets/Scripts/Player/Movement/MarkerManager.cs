using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerManager : MonoBehaviour
{
    public class Marker
    {
        public Vector3 position;
        public Quaternion rotation;

        public Marker(Vector3 pos, Quaternion rot)
        {
            position = pos;
            rotation = rot;
        }
    }
    public List<Marker> markerList = new List<Marker>();
    private bool update = true;
    private bool isHead = false;
    public void ChangeBoolean(int index, bool res) { 
        if (index == 0) { update = res; } 
        else            { isHead = res; } 
    }
    ////////////////////////////////////////////////////////////////////////////
    public void UpdateMarkerList()
    {
        markerList.Add(new Marker(transform.position, transform.rotation));
    }
    public void ClearMarkerList()
    {
        markerList.Clear();
        markerList.Add(new Marker(transform.position, transform.rotation));
    }

    ////////////////////////////////////////////////////////////////////////////
    private void FixedUpdate()
    {
        if (update && (GameManager.waveStart || SnakeManager.instance.forcedStart))
        {
            UpdateMarkerList();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isHead)
        {
            switch (collision.gameObject.tag)
            {
                case "WallT":
                case "WallB":
                    GetComponentInParent<SnakeManager>().SnakeWallBounce(Vector3.up);
                    break;

                case "WallR":
                case "WallL":
                    GetComponentInParent<SnakeManager>().SnakeWallBounce(Vector3.right);
                    break;
            }
        }
    }
}
