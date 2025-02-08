# adapted from https://www.geeksforgeeks.org/python-speech-recognition-on-large-audio-files/
import speech_recognition as sr
import time
from pydub import AudioSegment
from pydub import silence
import librosa
import soundfile as sf
import wave
import scipy.io as sio

# from textGen import generateRequestText

import os
import glob

listen_len = 2   # amount of speech to collect before sending to chatgpt api. measured in seconds. may change the value later
phrase_time_limit = 10.0  # max amount of time a "phrase" can be
max_audiofile_time = 60  # max amount of time an audio file can be

# transcribes at most listen_len seconds of speech, then sends it to the perplexity api
def transcribeAudio(fname):
    r = sr.Recognizer()
    list_of_phrases_raw = []

    # each wav is like at most 60 sec long

    audio_file_name = fname
    # x,_ = librosa.load(audio_file_name, sr=16000)
    # sf.write('tmp.wav', x, 16000)
    # wave.open('tmp.wav','r')

    # split the wav file
    audio = AudioSegment.from_wav(audio_file_name)
    # samplerate,audio = sio.wavfile.read(audio_file_name)
    phrases = silence.split_on_silence(audio, min_silence_len=600, silence_thresh=-32)

    # save all phrases into wav files
    i = 0
    # delete existing wav phrase files
    files = glob.glob('../phrases/*')
    for f in files:
        os.remove(f)

    # TODO: move these next 3 lines into the fn that calls transcribeAudio()
    # speech_trans_file = open("speech_transcription.txt", "w")
    # speech_trans_file.write("")
    # speech_trans_file.close()

    speech_trans_file = open("speech_transcription.txt", "a")

    for phrase in phrases:
        print(f"saving phrase{i}.wav") 

        # Create 0.5 seconds silence chunk 
        chunk_silent = AudioSegment.silent(duration = 10) 
  
        # add 0.5 sec silence to beginning and end of audio chunk. This is done so that 
        # it doesn't seem abruptly sliced. 
        phrase = chunk_silent + phrase + chunk_silent 

        # the name of the newly created chunk 
        os.makedirs("../phrases",exist_ok=True)
        phrase_audio_file_path = f"../phrases/phrase{i}.wav"  # file path relative to utils folder
        phrase.export(phrase_audio_file_path, bitrate='256k', format="wav") 
  
        print("Processing phrase " + str(i)) 
        with sr.AudioFile(f"{phrase_audio_file_path}") as source:
            # generate text for the phrase audio file
            list_of_phrases_raw.append(r.record(source, duration=phrase_time_limit))

        i = i + 1

    combined_phrases = ""
    j = 0
    for raw in list_of_phrases_raw:
        try:
            combined_phrases += (" " + r.recognize_google(raw) + ",")
        except:
            print(f"Phrase {j} not recognized")
        j = j + 1
    
    print(combined_phrases)

    # TODO: eventually uncomment the next 2 lines
    # gpt_response = generateRequestText(userQuery=combined_phrases)  #need to forward this to unity/frontend
    # gpt_response_text = gpt_response["text"]  
    gpt_response_text = "response"
    # TODO: if performance is bad, may want to try to write each phrase directly to the txt file instead of first appending to the combined phrases string
    speech_trans_file.write(f"audio transcription: {combined_phrases}\ngpt response: {gpt_response_text}\n\n")
    speech_trans_file.close()
    return combined_phrases







# # transcribes at most listen_len seconds of speech, then sends it to the perplexity api
# def transcribeAudio():
#     # Initialize recognizer class (for recognizing the speech)
#     r = sr.Recognizer()
#     # list_of_phrases = []
#     # arr_of_phrases = [None] * 5
#     list_of_phrases_raw = []

#     # Using microphone 
#     init_time = time.time()
#     # each wav is like at most 60 sec long
#     # while (time.time() - init_time < listen_len):
#         # with sr.Microphone() as source:
#         #     print("Talk")  # keeping these print statements here for debugging purposes for now
#         #     r.pause_threshold = 0.8
#         #     list_of_phrases_raw.append(r.listen(source, phrase_time_limit=phrase_time_limit))  #good?
#         #     print("Time over, thanks")

#     # using audio file from unity
#     # audio_file_name = "user_audio_file.wav"
#         # audio_file_name = "Jane_eyre.wav"
#         # with sr.AudioFile(f"../audio/{audio_file_name}") as source:   #TODO: Check with abby about audio file name
#         #     print("Talk")  # keeping these print statements here for debugging purposes for now
#         #     r.pause_threshold = 0.8
#         #     list_of_phrases_raw.append(r.record(source, duration=phrase_time_limit))  #good?
#         #     print("Time over, thanks")

#     for i in range(int(max_audiofile_time / phrase_time_limit) + 1):
#         audio_file_name = "Jane_eyre.wav"
#         with sr.AudioFile(f"../audio/{audio_file_name}") as source:   #TODO: Check with abby about audio file name
#             print("Talk")  # keeping these print statements here for debugging purposes for now
#             r.pause_threshold = 0.8
#             list_of_phrases_raw.append(r.listen(source, phrase_time_limit=phrase_time_limit))  #good?
#             print("Time over, thanks")

#     # list_of_phrases_text = []
#     combined_phrases = ""
#     for raw in list_of_phrases_raw:
#         try:
#             # list_of_phrases_text.append(r.recognize_google(raw))
#             combined_phrases += (" " + r.recognize_google(raw) + ",")
#         except:
#             print("Sorry, I did not get that")
    
#     print(combined_phrases)
#     # gpt_response = generateRequestText(userQuery=combined_phrases)  #need to forward this to unity/frontend

#     # add to text file as well
#     # gpt_response_text = gpt_response["text"]
#     gpt_response_text = "response"
#     speech_trans_file = open("speech_transcription.txt", "a")
#     speech_trans_file.write(f"audio transcription: {combined_phrases}\ngpt response: {gpt_response_text}\n\n")
#     speech_trans_file.close()

# transcribeAudio()
