using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip clipAntigravitySound;
    [SerializeField] private AudioClip clipTouchingGroundSound;
    [SerializeField] private AudioClip clipWalkSound;
    int i = 0;
    private AudioSource m_AudioAudioSource;

    private PlayerMovement m_PlayerMovement;
    private PlayerCollision m_PlayerCollision;

    // Start is called before the first frame update
    private void Start()
    {
        m_PlayerMovement = GetComponent<PlayerMovement>();
        m_AudioAudioSource = GetComponent<AudioSource>();
        m_PlayerCollision = GetComponent<PlayerCollision>();
    }

    // Update is called once per frame
    private void Update()
    {
        ApplyWalkingSoundEffect();
        ApplyJumpSoundEffect();
        //ApplyTouchingGroundSoundEffect();
    }

    private void ApplyWalkingSoundEffect()
    {
        if (m_PlayerMovement.IsMoving && !m_PlayerCollision.OnWallCollision)
        {
            if (!m_AudioAudioSource.isPlaying)
            {
                m_AudioAudioSource.PlayOneShot(clipWalkSound);
                
            }
        }
        else
        {
            return;
        }
    }

    private void ApplyJumpSoundEffect()
    {
        if (m_PlayerMovement.IsJumping && !m_PlayerCollision.OnGroundCollision)
        {
            
            m_AudioAudioSource.PlayOneShot(clipAntigravitySound,0.5f);
            
        }
        else
        {            
            return;
        }
    }

    private void ApplyTouchingGroundSoundEffect()
    {
        if (m_PlayerCollision.OnGroundCollision)
        {
            if (!m_AudioAudioSource.isPlaying)
            {
                m_AudioAudioSource.PlayOneShot(clipTouchingGroundSound);
            }
        }
        else
        {            
            return;
        }
    }
}