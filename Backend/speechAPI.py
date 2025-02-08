from fastapi import FastAPI,Response
from pydantic import BaseModel

from utils import (audioGen,textGen,audioStore)

app = FastAPI()

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
    questionText = textGen.generateRequestText(
        config.agentRole,
        config.agentTone,
        config.userRole,
        config.userQuery
    )
    if not questionText['ok']:
        print("error loading question text.\n")
        print(questionText['error'])
        return {"ok": False, "error": questionText['error'],"data":""}
    questionAudio = audioGen.generateAudio(questionText['text'])
    return Response(content=questionAudio,media_type="audio/mp3")


@app.post("/store-user-resp")
async def store_user_response(userResponse: UserAudio):
    '''
    input: user audio object
    returns: transcription status update
    '''
    transcription = audioStore.transcribeAudio()
    pass