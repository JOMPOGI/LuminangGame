using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls a single row in the Leaderboard list (top 10 or current player rank).
/// </summary>
public class LeaderboardRowItem : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI rankText;
    public Image badgeImage;
    public Image avatarImage;
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI progressText;
    public Button rowButton;

    // Badges for Top 3 (can be passed in during setup)
    [Header("Badge Setup")]
    public Sprite goldBadge;
    public Sprite silverBadge;
    public Sprite bronzeBadge;
    [Tooltip("Special badge shown on the current player's own row (the 'Your Rank' circle).")]
    public Sprite currentPlayerBadge;

    [Header("Text Coloring")]
    [Tooltip("Color of the rank text when the player is in the top 3 (usually white or styled by user).")]
    public Color top3TextColor = Color.white;
    [Tooltip("Color of the rank text when the player is not in the top 3.")]
    public Color normalTextColor = Color.black;

    private LeaderboardPlayer _playerData;
    private LeaderboardManager _manager;
    private int _rank;
    private ColorBlock _originalColors;
    private bool _colorsStored = false;

    public LeaderboardPlayer PlayerData => _playerData;

    public void Setup(LeaderboardPlayer player, int rank, LeaderboardManager manager)
    {
        _playerData = player;
        _rank = rank;
        _manager = manager;

        // Set Rank Number & Badge
        if (rankText != null)
        {
            rankText.text = rank.ToString();
            rankText.gameObject.SetActive(true); // Keep it active so it displays ON TOP of badges

            // Apply coloring based on whether they are in the top 3 or the current player
            if (rank <= 3 || player.is_current_player)
            {
                rankText.color = top3TextColor;
            }
            else
            {
                rankText.color = normalTextColor;
            }
        }

        if (badgeImage != null)
        {
            // Current player always gets their own special badge regardless of rank
            if (player.is_current_player && currentPlayerBadge != null)
            {
                badgeImage.sprite = currentPlayerBadge;
                badgeImage.gameObject.SetActive(true);
            }
            else if (rank == 1 && goldBadge != null)
            {
                badgeImage.sprite = goldBadge;
                badgeImage.gameObject.SetActive(true);
            }
            else if (rank == 2 && silverBadge != null)
            {
                badgeImage.sprite = silverBadge;
                badgeImage.gameObject.SetActive(true);
            }
            else if (rank == 3 && bronzeBadge != null)
            {
                badgeImage.sprite = bronzeBadge;
                badgeImage.gameObject.SetActive(true);
            }
            else
            {
                // Deactivate the badge completely so no shadow/backplate remains visible
                badgeImage.gameObject.SetActive(false);
            }
        }

        // Set Username & Progress
        if (usernameText != null)
            usernameText.text = player.username;

        if (progressText != null)
            progressText.text = $"{player.overall_progress:F1}%";

        // Set Avatar (or default white image if null)
        if (avatarImage != null)
        {
            if (!string.IsNullOrEmpty(player.picture))
            {
                // Load custom sprite...
            }
            else
            {
                avatarImage.color = Color.white; // Default white picture
            }
        }

        // Click Handler
        if (rowButton != null)
        {
            rowButton.onClick.RemoveAllListeners();
            rowButton.onClick.AddListener(OnClick);
        }
    }

    public void SetSelected(bool isSelected)
    {
        if (rowButton != null)
        {
            if (!_colorsStored)
            {
                _originalColors = rowButton.colors;
                _colorsStored = true;
            }

            ColorBlock cb = rowButton.colors;
            if (isSelected)
            {
                cb.normalColor = _originalColors.selectedColor;
                cb.highlightedColor = _originalColors.selectedColor;
            }
            else
            {
                cb.normalColor = _originalColors.normalColor;
                cb.highlightedColor = _originalColors.highlightedColor;
            }
            rowButton.colors = cb;
        }
    }

    private void OnClick()
    {
        if (_manager != null && _playerData != null)
        {
            _manager.SelectRow(this, _playerData, _rank);
        }
    }
}
