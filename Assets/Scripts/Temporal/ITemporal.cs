public interface ITemporal
{
    int ExecutionOrder();
    void Initialize(int bufferSize);
    void UpdateTemporalState(int currentFrame, bool reversing);
    void ResetTemporal();
    void SetActive(bool value);
    void StartedReversing();
}