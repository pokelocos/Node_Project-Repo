using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RA.DialogSystem
{
    public class DialogView : MonoBehaviour
    {
        public delegate void dialogEvent();
        public dialogEvent OnStart;
        public dialogEvent OnAbort;
        public dialogEvent OnEnd;

        public dialogEvent OnStartSentence;
        public dialogEvent OnEndSentence;

        public Text displayText;
        public Text displayName;
        public Image displayImage;

        /// <summary>
        /// Starts an asynchronous routine and returns it.
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="typingSpeed"></param>
        /// <returns></returns>
        public Coroutine ShowSentence(Sentence sentence, float typingSpeed)
        {
            return StartCoroutine(TypeTheSentence(sentence, typingSpeed));
        }

        /// <summary>
        /// Show a sentence.
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="typingSpeed"></param>
        /// <returns></returns>
        private IEnumerator TypeTheSentence(Sentence sentence, float typingSpeed)
        {
            OnStartSentence?.Invoke();
            SetView(sentence.actor.name, sentence.actor.GetSprite(-1), ""); // comentado solo para este projecto
            //SetView(sentence.actor.name, sentence.actor.GetSprite((int)sentence.emotion), "");
            foreach (char letter in sentence.text.ToCharArray())
            {
                displayText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }
            OnEndSentence?.Invoke();
        }

        /// <summary>
        /// Set values whit the snetence information.
        /// </summary>
        public void SetView(Sentence sentece)
        {
            displayName.text = sentece.actor.name;
            displayImage.sprite = sentece.actor.GetSprite(-1); // comentado solo para este projecto
            //displayImage.sprite = sentece.actor.GetSprite((int)sentece.emotion);
            displayText.text = sentece.text;
        }

        /// <summary>
        /// Set values whit basic information
        /// </summary>
        /// <param name="name"></param>
        /// <param name="img"></param>
        /// <param name="text"></param>
        public void SetView(string name, Sprite img, string text)
        {
            displayName.text = name;
            displayImage.sprite = img;
            displayText.text = text;
        }

        /// <summary>
        /// set values whit generic blank values
        /// </summary>
        public void ClearView()
        {
            SetView("", null, "");
        }
    }
}
