using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Pathfinding;
using System.Linq;
using TMPro;

public partial class MyUnit : MonoBehaviour
{
    
    public LayerMask targetLayer;
    public LayerMask detectLayer;
    #region Property
        public bool IsAttackable {get => _isAttackable; set => _isAttackable = value;}
        bool _isAttackable = true;
        public bool IsSpecialAttackable {get => _isSpecialAttackable; set => _isSpecialAttackable = value;}
        bool _isSpecialAttackable = false;
    #endregion
    #region Status
        public float attackRadius = 2f;
        public float detectRadius = 5f;
        float _moveVelocity = 1f;
    #endregion
    #region Component
        Rigidbody2D _rigid;
        Collider2D _collider;
    #endregion
    #region MoveControlMembers
        Seeker _seeker;
        int _divisionNum = 24;
        float _unitDegree;
        Vector3[] _normalDirections;
        float[] _currentDirectionWeights;
        List<Vector3> _weightList;
        Vector3 _destination;
    #endregion
   
    private void Awake() {
        _rigid = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();

        #region MoveControlInit
        _seeker = GetComponent<Seeker>();

        _normalDirections = new Vector3[_divisionNum];
        _currentDirectionWeights = new float[_divisionNum];
        _weightList = new List<Vector3>();
        
        _unitDegree = 2 * Mathf.PI / _divisionNum;
        for (int i = 0; i < _divisionNum; ++i){
            _normalDirections[i] = new Vector3(Mathf.Cos(_unitDegree * i), Mathf.Sin(_unitDegree * i), 0);
        }
        #endregion
    }   

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
    public virtual GameObject AISelectAttackTarget(Collider2D[] detectEnemies_){
        //nearest
        float minDist = Mathf.Infinity;
        Collider2D nearestEnemy = null;
        foreach (var enemy in detectEnemies_)
        {
            
            if(enemy == null) break;
            float dist = _collider.Distance(enemy).distance;
            if (dist < minDist) {
                minDist = dist;
                nearestEnemy = enemy;
            }
        }
        if(nearestEnemy != null){
            return nearestEnemy.gameObject;
        } 
        return null;
    }
    public virtual bool AIAttack(){
        if(IsAttackable){
            IsAttackable = false;
            GetComponent<SpriteRenderer>().color = Color.red;
            StartCoroutine(AttackRoutine());
            return true;
        }
        return false;
    }
    IEnumerator AttackRoutine(){
        yield return new WaitForSeconds(0.5f);
        GetComponent<SpriteRenderer>().color = Color.white;
        IsAttackable = true;
    }
    public virtual bool AISpecialAttack(){
        if(IsSpecialAttackable){
            IsSpecialAttackable = false;
            GetComponent<SpriteRenderer>().color = Color.yellow;
            StartCoroutine(SpecialAttackRoutine());
            return true;
        }
        return false;
    }
    IEnumerator SpecialAttackRoutine(){
        yield return new WaitForSeconds(0.5f);
        GetComponent<SpriteRenderer>().color = Color.white;
        IsSpecialAttackable = true;
    }
    public virtual void AIDetectAround(Collider2D[] detectThings_){
        //nearest
        float minDist = Mathf.Infinity;
        Collider2D nearestEnemy = null;
        foreach (var thing in detectThings_)
        {
            if(thing == null) break;
            float dist = _collider.Distance(thing).distance;
            if (dist < minDist) {
                minDist = dist;
                nearestEnemy = thing;
            }
        }
        if(nearestEnemy == null){
            return;
        } 
        FindPathToDesitnation(nearestEnemy.transform.position);
        AddWeight(_destination - transform.position);
        // Move(CalculateDirections(0.2f));
    }
    public virtual bool AIMove(){
        Vector3 direction = CalculateDirections(0.2f);
        _rigid.velocity = _moveVelocity * direction;
        if(direction == Vector3.zero) return false;
        return true;
    }
    public virtual bool AISpecialMove(){
        return false;
    }

    #region MoveControl
    #region AstarPAthFinding
    void FindPathToDesitnation(Vector3 destination_){
        Path p = _seeker.StartPath (transform.position, destination_, OnPathComplete);
        p.BlockUntilCalculated();
    }
    public void OnPathComplete (Path p) {
        if (p.error) {
            _destination = transform.position;
            Debug.Log("Nooo, a valid path couldn't be found");
        } else {
            _destination = p.vectorPath[1];
        }
    }        

    #endregion
    void AddWeight(Vector3 vector_){
        _weightList.Add(vector_);
    }
    void AddWeight(Vector3 direction_, float weight){
        _weightList.Add(weight * direction_.normalized);
    }
    protected Vector3 CalculateDirections(float threshold_ = 0){
        for (int i = 0; i < _divisionNum; ++i){
            _currentDirectionWeights[i] = 0;
        }
        int maxIdx = 0;
        for (int i = 0; i < _divisionNum; ++i){
            foreach (var weight in _weightList){
                _currentDirectionWeights[i] += Vector3.Dot(_normalDirections[i], weight);
            }
            if(_currentDirectionWeights[i] > _currentDirectionWeights[maxIdx]){
                maxIdx = i;
            }
        } 
        _weightList.Clear();
        if(Mathf.Abs(_currentDirectionWeights[maxIdx]) <= threshold_) 
            return Vector3.zero;
        
        // normalize for gizmo
        if(_currentDirectionWeights[maxIdx] != 0){
            for (int i = 0; i < _divisionNum; ++i){
                _currentDirectionWeights[i] = _currentDirectionWeights[i] / _currentDirectionWeights[maxIdx];
            } 
        }

        return _normalDirections[maxIdx];
    }

    void OnDrawGizmos()
    {
        if(_currentDirectionWeights != null && _currentDirectionWeights.Length == _divisionNum && _currentDirectionWeights.Sum() != 0){
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
            for (int i = 0; i < _divisionNum; ++i){
                if(_currentDirectionWeights[i] >= 0){
                    Gizmos.color = Color.green;
                }
                else{
                    Gizmos.color = Color.red;
                }
                Gizmos.DrawLine(transform.position + 0.5f * _normalDirections[i], transform.position + (0.5f + Mathf.Abs(_currentDirectionWeights[i])) * _normalDirections[i]);
            } 
        }
    }
        
    #endregion
}
