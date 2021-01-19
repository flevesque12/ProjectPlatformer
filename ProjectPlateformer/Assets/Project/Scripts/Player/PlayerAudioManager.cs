using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip clipAntigravitySound;
    [SerializeField] private AudioClip clipTouchingGroundSound;
    [SerializeField] private AudioClip clipWalkSound;

    private AudioSource m_AudioAudioSource;

    private PlayerMovement m_PlayerMovement;

    // Start is called before the first frame update
    private void Start()
    {
        m_PlayerMovement = GetComponent<PlayerMovement>();
        m_AudioAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void Update()
    {
        ApplySoundWalkingEffect();
        ApplyAntigraviyActivationSoundEffect();
        //ApplyTouchingGroundSoundEffect();
    }

    private void ApplySoundWalkingEffect()
    {
        if (m_PlayerMovement.IsMoving)
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

    private void ApplyAntigraviyActivationSoundEffect()
    {
        if (m_PlayerMovement.IsJumpStart)
        {
            m_AudioAudioSource.PlayOneShot(clipAntigravitySound);
        }
    }

    private void ApplyTouchingGroundSoundEffect()
    {
        if (m_PlayerMovement.IsOnFloor)
        {
            m_AudioAudioSource.PlayOneShot(clipTouchingGroundSound);
        }
        else
        {
            return;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        m_AudioAudioSource.PlayOneShot(clipTouchingGroundSound);
    }
}