from pathlib import Path
from openai import OpenAI
import zipfile
import os

from pydub import AudioSegment

client = OpenAI(api_key="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")





def generateAudio(text,fp = "./generated_audio_transcripts"):
    ''' 
    input: text to pronounce
    returns: AudioFile (.wav type)
    '''
    response = client.audio.speech.create(
    model="tts-1",
    voice="alloy",
    input=text,
    )


    os.makedirs(fp,exist_ok=True)
    response.stream_to_file(fp+"/test.mp3")
    
    audioZip = zipfile.ZipFile(fp+"/test.zip", "w", zipfile.ZIP_DEFLATED)
    audioZip.write(fp+"/test.mp3")
    audioZip.close()


    # print(response.content)
    return response.content