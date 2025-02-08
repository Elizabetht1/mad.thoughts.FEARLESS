import speech_recognition as sr
import time

# TODO:
# store text locally for redundancy
# make this function also send to api

listen_len = 20   # may change later

# transcribes at most listen_len seconds of speech, then sends it to the perplexity api
def transcribeAudio():
    # Initialize recognizer class (for recognizing the speech)
    r = sr.Recognizer()
    # list_of_phrases = []
    list_of_phrases_raw = []
    # arr_of_phrases = [None] * 5
    # i = 0 # index into arr_of_phrases

    init_time = time.time()
    while (time.time() - init_time < listen_len):
        with sr.Microphone() as source:
            print("Talk")  # keeping these print statements here for debugging purposes
            r.pause_threshold = 0.8
            list_of_phrases_raw.append(r.listen(source, phrase_time_limit=10.0))  #good?
            print("Time over, thanks")

    list_of_phrases_text = []
    for raw in list_of_phrases_raw:
        try:
            list_of_phrases_text.append(r.recognize_google(raw))
        except:
            print("Sorry, I did not get that")

