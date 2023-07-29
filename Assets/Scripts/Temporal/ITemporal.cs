public interface ITemporal
{
    void UpdateTemporalState(int currentFrame, bool reversing);
    void ResetTemporal();
    void SetActive(bool value);
}