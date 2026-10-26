using System;
using UnityEngine;

public class MinigameSaveable : MonoBehaviour, ISaveable
{
    [SerializeField] private string _saveId = "bird_nest_minigame";

    [SerializeField] private NestMinigame _nest;
    [SerializeField] private BigBug _bigBug;
    [SerializeField] private GreenBugDrag _greenBugDrag;
    [SerializeField] private BranchManager _branchManager;
    [SerializeField] private BranchPickup _branchPickup;
    [SerializeField] private TreeSequence _treeSequence;
    [SerializeField] private BugClick[] _bugs;
    public string SaveId => _saveId;

    [Serializable]
    private class MinigameState
    {
        public int eggClickCount;
        public bool bugCollected;
        public bool birdsFed;
        public bool branchMoved;
        public bool branchPickedUp;
        public Vector3 branchPosition;
        public int treeHits;
        public bool treeBirdsShown;
        public bool jasonFalling;

        public Vector3 jasonPosition;
        public bool jasonActive;

        public Vector3 birdPosition;
        public bool birdActive;

        public bool areaBlockerActive;
        public bool[] bugsSquished;
    }

    public string CaptureState()
    {
        // Collect all progress from this minigame.
        MinigameState state = new MinigameState();

        state.eggClickCount = _nest.GetEggClickCount();
        state.bugCollected = _bigBug.IsBugCollected;
        state.birdsFed = GreenBugDrag.birdsFed;

        // Save the branch's current state.
        state.branchMoved = _branchManager.HasBranchMoved;
        state.branchPickedUp = _branchPickup.IsHoldingBranch();
        state.branchPosition = _branchManager.branchImage.transform.position;

        // Save the tree puzzle's internal progress.
        state.treeHits = _treeSequence.HitCount;
        state.treeBirdsShown = _treeSequence.BirdsShown;
        state.jasonFalling = _treeSequence.IsJasonFalling;

        // Save Jason's current position and visibility.
        state.jasonPosition = _treeSequence.jason.transform.position;
        state.jasonActive = _treeSequence.jason.activeSelf;

        // Only save the bird if one is assigned.
        if (_treeSequence.bird != null)
        {
            state.birdPosition = _treeSequence.bird.transform.position;
            state.birdActive = _treeSequence.bird.activeSelf;
        }

        // Save whether the blocked area is still closed.
        state.areaBlockerActive = _treeSequence.areaBlocker.activeSelf;

        // Save each bug's squished state.
        state.bugsSquished = new bool[_bugs.Length];

        for (int i = 0; i < _bugs.Length; i++)
        {
            state.bugsSquished[i] = _bugs[i].IsSquished;
        }

        return JsonUtility.ToJson(state);
    }

    public void RestoreState(string json)
    {
        // Read the saved minigame progress.
        MinigameState state = JsonUtility.FromJson<MinigameState>(json);

        // Restore each part of the puzzle.
        _nest.RestoreEggClickCount(state.eggClickCount);
        _bigBug.RestoreBugCollected(state.bugCollected);
        _greenBugDrag.RestoreBirdFeeding(state.birdsFed);

        // Restore the branch's position and internal state.
        _branchManager.RestoreBranchMoved(state.branchMoved);
        _branchManager.branchImage.transform.position = state.branchPosition;

        // Restore whether Lona is holding the branch.
        _branchPickup.RestoreHoldingBranch(state.branchPickedUp);

        // Restore the internal tree puzzle state.
        _treeSequence.RestoreTreeState
        (
            state.treeHits,
            state.treeBirdsShown,
            state.jasonFalling
        );

        // Restore Jason's position and visibility.
        _treeSequence.jason.transform.position = state.jasonPosition;
        _treeSequence.jason.SetActive(state.jasonActive);

        // Only restore the bird if one is assigned.
        if (_treeSequence.bird != null)
        {
            _treeSequence.bird.transform.position = state.birdPosition;
            _treeSequence.bird.SetActive(state.birdActive);
        }

        // Restore whether the player can enter the next area.
        _treeSequence.areaBlocker.SetActive(state.areaBlockerActive);

        // Restore the appearance of each bug.
        if (state.bugsSquished != null)
        {
            int count = Mathf.Min(_bugs.Length, state.bugsSquished.Length);

            for (int i = 0; i < count; i++)
            {
                _bugs[i].RestoreSquished(state.bugsSquished[i]);
            }
        }
    }
}
