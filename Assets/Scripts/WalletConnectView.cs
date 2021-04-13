using System;
using System.Collections;
using System.Collections.Generic;
//using Nethereum.JsonRpc.UnityClient;
//using Nethereum.StandardTokenEIP20.ContractDefinition;
using Nethereum.Web3;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using UnityEngine;
using System.Numerics;
using System.Threading.Tasks;
using System.Text;
using System.Net;
using WalletConnectSharp;
using WalletConnectSharp.Models;

public class WalletConnectView : MonoBehaviour
{
    WalletConnect walletConnect;

    public WalletLinkView view;

    [Header("Dapp Metadata")]
    public string dappDescription = "This is a test of the Nethereum.WalletConnect feature";
    public string dappIcon = "https://app.warriders.com/favicon.ico";
    public string dappName = "WalletConnect Test";
    public string dappUrl = "https://app.warriders.com";

    public void ConnectToWallet()
    {
        this.AyncWalletConnectStart();
    }

    private async void AyncWalletConnectStart()
    {
        var metadata = new ClientMeta()
        {
            Description = dappDescription,
            Icons = new[] { dappIcon },
            Name = dappName,
            URL = dappUrl
        };

        walletConnect = new WalletConnect(metadata);

        view.SetLink(walletConnect.URI);

        view.gameObject.SetActive(true);

        try
        {
            walletConnect.OnConnect += OnConnect;
            await walletConnect.Connect();

            Debug.Log("Disconnected");
        }
        catch (Exception e)
        {
            Debug.Log("An error occurred: " + e.Message);
            Debug.Log(e.StackTrace);
        }

    }

    private void OnConnect(object walletConnect, WalletConnect result)
    {
        Debug.Log("Account: " + result.Accounts[0]);
        Debug.Log("chainId: " + result.ChainId);
    }

    void Update()
    {
        if (walletConnect != null)
        {
            walletConnect.DispatchMessageQueue();
        }
    }

}
