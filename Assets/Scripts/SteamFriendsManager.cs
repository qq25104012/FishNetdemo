using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using TMPro;
using UnityEngine.UI;

public class SteamFriendsManager : MonoBehaviour
{
    [SerializeField] private RawImage profilePicture;
    [SerializeField] TextMeshProUGUI playerName;

    [SerializeField] private Transform friendsContent;
    [SerializeField] private GameObject friendsItem;

    private async void Start()
    {
        if (!SteamClient.IsValid)
        {
            return;
        }

        playerName.text = SteamClient.Name;

        InitFriends();

        var img = await SteamFriends.GetLargeAvatarAsync(SteamClient.SteamId);
        profilePicture.texture = GetTextureFromImage(img.Value);
    }

    public static Texture2D GetTextureFromImage(Steamworks.Data.Image _image)
    {
        Texture2D texture = new Texture2D((int)_image.Width, (int)_image.Height);

        for (int x = 0; x < _image.Width; x++)
        {
            for (int y = 0; y < _image.Height; y++)
            {
                var p = _image.GetPixel(x, y);
                texture.SetPixel(x, (int)_image.Height - y, new Color(p.r / 255.0f, p.g / 255.0f, p.b / 255.0f, p.a / 255.0f));
            }
        }

        texture.Apply();
        return texture;
    }

    private async void InitFriends()
    {
        foreach (var friend in SteamFriends.GetFriends())
        {
            if (friend.IsOnline)
            {
                GameObject friendItem = Instantiate(friendsItem, friendsContent);

                var img = await SteamFriends.GetLargeAvatarAsync(friend.Id);
                friendItem.GetComponent<FriendItem>().Setup(GetTextureFromImage(img.Value), friend.Name, friend.Id);
            }
        }
    }
}
