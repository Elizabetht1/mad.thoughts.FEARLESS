from pathlib import Path
from openai import OpenAI
import zipfile
import os

import shutil


from pydub import AudioSegment

client = OpenAI(api_key="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")


from pathlib import Path


def generateAudio(text,fp = "./assets"):
    ''' 
    input: text to pronounce
    returns: AudioFile (.wav type)
    '''
    print(text)

    fullDest = Path("assets").parent / "test.mp3"

    response = client.audio.speech.create(
        model= "tts-1",
        voice="alloy",
        input=text,
    )

    FNAME = "/test.mp3"
    ASSETS_URL = "http://127.0.0.1:8000/assets"
    
    os.makedirs(fp,exist_ok=True)
    response.stream_to_file(fullDest)
    
    audioZip = zipfile.ZipFile(fp + FNAME, "w", zipfile.ZIP_DEFLATED)
    audioZip.write(fp + FNAME)
    audioZip.close()

    shutil.copyfile("./test.mp3", "./assets/test.mp3")
    # print(response.content)
    return ASSETS_URL + FNAME