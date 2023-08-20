using DG.Tweening;
using UnityEngine;

namespace CustomUITween
{
    public static class DOTweenUIAnimationFactory
    {
        public static BobbleAndWave BobbleAndWave(GameObject gameObject, float positionY, float eulerZ, bool pause = false) => new BobbleAndWave(gameObject, positionY, eulerZ, pause);
    }


    public class BobbleAndWave
    {
        private float PositionY = 0f;
        private float EulerZ = 0f;

        private GameObject GameObject;

        private Tweener Bobble;
        private Tweener Wave;

        public BobbleAndWave(GameObject gameObject, float positionY, float eulerZ, bool pause = false)
        {
            PositionY = positionY;
            EulerZ = eulerZ;
            GameObject = gameObject;

            Bobble = GameObject.transform.DOLocalMoveY(GameObject.transform.localPosition.y - (PositionY / 2), 1.27f)
                .From(GameObject.transform.localPosition.y + (PositionY / 2))
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);

            Wave = GameObject.transform
                .DOLocalRotate(new Vector3(0, 0, GameObject.transform.localEulerAngles.z + (EulerZ / 2)), 1f)
                .From(new Vector3(0, 0, GameObject.transform.localEulerAngles.z - (EulerZ / 2)))
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);

            if (pause)
            {
                Bobble.Pause();
                Wave.Pause();
            }
        }

        public void Play()
        {
            Bobble.Play();
            Wave.Play();
        }

        public void Restart()
        {
            Bobble.Restart();
            Wave.Restart();
        }
    }
}

