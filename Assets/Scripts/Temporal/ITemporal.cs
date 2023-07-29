public interface ITemporal
{
    public int ExecutionOrder();
    void UpdateTemporalState(int currentFrame, bool reversing);
    void ResetTemporal();
    void SetActive(bool value);
}