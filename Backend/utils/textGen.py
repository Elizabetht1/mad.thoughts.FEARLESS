import openai
import os
import logging


api_key = os.environ["OPENAI_API_KEY"]

client = openai.OpenAI(api_key=api_key, base_url="https://api.perplexity.ai")

logger = logging.getLogger(__name__)

def constructMsg(
    agentRole = "interviewer",
    agentTone = "confused",
    userRole = "interviewee",
    userQuery = None
):
    if not userQuery:
        return {"ok":False, "error":"no user string provided", "messages":[]}
    systemMsg = {
        "role": "system",
        "content": (
            f"You are a {agentRole} and you need to "
            f"engage in a {agentTone} conversation with a {userRole}."
        ),
    }
    userMsg = {   
        "role": "user",
        "content": (
           userQuery
        ),
    } 
    return {"ok":True, "error":"", "messages":[systemMsg,userMsg]}

def generateRequestText(
    agentRole = "interviewer",
    agentTone = "confused",
    userRole = "interviewee",
    userQuery = None
):
    if not userQuery:
        return {"ok":False, "error":"no user string provided", "text":""}
    logging.basicConfig(filename='./logging/textGen.log', level=logging.INFO)

    ok,error,messages = constructMsg(agentRole,agentTone,userRole, userQuery).values()
   
    if not ok:
        logger.info(f"error with message generation: \n {error} \n")
        return {"ok": False, "error": f"error with message generation: \n {error}", "text":""}
    

    response = client.chat.completions.create(
        model="sonar",
        messages=messages,
        stream = False                                   ##TODO FIGURE OUT HOW TO STREAM THINGS
    )
    
        
    logger.info(f"received a response from perplexity. \n"
                    f"completion_tokens_used: {response.usage.completion_tokens} \n"
                    f"prompt_tokens_used: {response.usage.prompt_tokens} \n"
                    f"total_tokens_used: {response.usage.total_tokens} \n"
                    )
    for choice in response.choices:
            logger.info("potential choice from perplexity\n")
            logger.info(choice)

    res = response.choices[0].message.content
    return {"ok": True, "error": "", "text":res}
    