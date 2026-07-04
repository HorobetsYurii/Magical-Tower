using UnityEngine;

public class TowerView : MonoBehaviour
{
    [SerializeField] private TowerHealthPresenter healthPresenter;

    public void Bind(Tower tower)
    {
        healthPresenter.Bind(tower.Health);
    }
}
