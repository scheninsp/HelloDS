using System;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;

public class NetworkCommandLine : MonoBehaviour
{
    public static NetworkManager netManager;

    void Start()
    {
        netManager = GetComponentInParent<NetworkManager>();

        if (Application.isEditor)
        {
            Debug.Log("Starting client (editor mode) ...");
            netManager.StartClient();
            return;
        }

        var args = GetCommandlineArgs();

        if (args.TryGetValue("-mode", out string mode))
        {
            switch (mode)
            {
                case "server":
                    Debug.Log("Starting server ...");
                    netManager.StartServer();
                    break;
                case "host":
                    Debug.Log("Starting host ...");
                    netManager.StartHost();
                    break;
                case "client":
                    Debug.Log("Starting client ...");
                    netManager.StartClient();
                    break;
            }
        }
        else
        {
            Debug.Log("Starting client (default mode) ...");
            netManager.StartClient();
        }
    }

    private Dictionary<string, string> GetCommandlineArgs()
    {
        Dictionary<string, string> argDictionary = new Dictionary<string, string>();

        var args = System.Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length; ++i)
        {
            var arg = args[i].ToLower();
            if (arg.StartsWith("-"))
            {
                var value = i < args.Length - 1 ? args[i + 1].ToLower() : null;
                value = (value?.StartsWith("-") ?? false) ? null : value;

                argDictionary.Add(arg, value);
            }
        }
        return argDictionary;
    }
}
