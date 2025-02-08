from fastapi import FastAPI,Response, File, UploadFile
from pydantic import BaseModel
from fastapi.staticfiles import StaticFiles
import shutil

from utils import (audioGen,textGen,audioStore)
from typing import Annotated
import os 

app = FastAPI()
app.mount("/assets", StaticFiles(directory="./assets"), name="assets")

''' DATA CLASSES '''
class ConvoConfig(BaseModel):
    agentRole: str
    agentTone: str
    userRole: str
    userQuery: str


class UserAudio(BaseModel):
    pass




''' ENDPOINTS '''
@app.post("/gen-convo")
async def generate_conversation(config: ConvoConfig)->str:
    '''
    input: enviornment configuration
    returns: AudioFile (.wav type)
    '''
    questionText = textGen.generateText(
        config.agentRole,
        config.agentTone,
        config.userRole,
        config.userQuery
    )
    if not questionText['ok']:
        print("error loading question text.\n")
        print(questionText['error'])
        return {"ok": False, "error": questionText['error'],"data":""}
    questionAudioURL = audioGen.generateAudio(questionText['text'])
    
    # resp = Response(content=questionAudio,media_type="audio/mp3")
    return Response(content=questionAudioURL)


@app.post("/transcribe-user-speech")
async def transcribe_user_speech(file: UploadFile):
    '''
    input: user audio object
    returns: transcription status update
    '''
    os.makedirs("./user_audio_files",exist_ok = True)
    with open(f"./user_audio_files/{file.filename}","wb") as buffer:
        shutil.copyfileobj(file.file, buffer)

    transcription = audioStore.transcribeAudio(f"./user_audio_files/{file.filename}")
    print(transcription)
    return transcription