from fastapi import FastAPI
from pydantic import BaseModel

from utils import (audioGen,textGen,audioStore)

app = FastAPI()

''' DATA CLASSES '''
class ConvoConfig(BaseModel):
    pass

class UserAudio(BaseModel):
    pass

''' ENDPOINTS '''

@app.post("/gen-convo")
def generate_conversation(config: ConvoConfig):
    '''
    input: enviornment configuration
    returns: AudioFile (.wav type)
    '''
    questionText = textGen.generateText()
    questionAudio = audioGen.generateAudio()
    pass


@app.post("/store-user-resp")
def store_user_response(userResponse: UserAudio):
    '''
    input: user audio object
    returns: transcription status update
    '''
    transcription = audioStore.transcribeAudio()
    pass