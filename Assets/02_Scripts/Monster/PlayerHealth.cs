using UnityEngine;

public class PlayerHealth : CharacterModelBase, IDamageable
{

    float h; 
    float v;

    private void Start()
    {
        _currentHealth = maxHealth;
        DebuffHandler handler = gameObject.GetComponent<DebuffHandler>();
        handler.InitializePlayer();
    }

    private void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        transform.Translate(h * Time.deltaTime * 5, 0, v * Time.deltaTime * 5);
    }
}
