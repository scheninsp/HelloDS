using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Move();
        }
    }

    public void Move()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            var randomPosition = GetRandomPositionOnPlane();
            transform.position = randomPosition;
            Position.Value = randomPosition;
        }
        else
        {
            SubmitPositionRequestServerRpc();
        }
    }

    [ServerRpc]
    void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
    {
        Position.Value = transform.position;
    }

    static Vector3 GetRandomPositionOnPlane()
    {
        return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
    }


    bool m_moveUp = true;
    float m_moveLimit = 5.0f;
    [SerializeField]
    public float m_moveSpeed = 0.05f;
    void Update()
    {
        if (IsServer)
        { // server, move player up-side-down
            var oldPosition = transform.position;
            if (m_moveUp)
            {
                if (oldPosition.y > m_moveLimit)
                {
                    m_moveUp = false;
                }
            }
            else
            {
                if (oldPosition.y < -m_moveLimit)
                {
                    m_moveUp = true;
                }
            }
            float moveDelta = m_moveUp ? m_moveSpeed : -m_moveSpeed;
            transform.position = new Vector3(oldPosition.x, oldPosition.y + moveDelta, oldPosition.z);
            Position.Value = transform.position;
        } 
        else
        { // client, sync position from server only
            transform.position = Position.Value;
        }
    }
}
