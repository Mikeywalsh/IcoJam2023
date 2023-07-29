public interface ITemporal
{
    void UpdateTemporalState();
    void ResetTemporal();
    void SetActive(bool value);
}