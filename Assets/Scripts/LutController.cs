using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class LutController : MonoBehaviour
{
    public Volume globalvolume;
    public List<Texture> luts = new List<Texture>();
    private ColorLookup colorLookup;
    public GameObject Door;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!globalvolume.profile.TryGet(out colorLookup))
        {

        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            colorLookup.texture.value = luts[0];
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            colorLookup.texture.value = luts[1];
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            colorLookup.texture.value = luts[2];
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            colorLookup.texture.value = luts[3];
        }
    }

    public void SetHorror()
    {
        colorLookup.texture.value = luts[2];
    }
    public void SetOff()
    {
        colorLookup.texture.value = luts[3];
        Door.SetActive(false);
    }
}




