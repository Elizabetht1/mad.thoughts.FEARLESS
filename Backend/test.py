import requests

import asyncio

url = 'http://127.0.0.1:8000'
myobj = {'agentRole': 'interviewer',
         'agentTone': 'neutral',
         'userRole' : 'job applicant',
         'userQuery': 'hello, would you please give me a job?'
         }

''' test gen convo '''
async def run_response():
    resp = requests.post(url+"/gen-convo", json = myobj)
    print(resp.content)
    return
asyncio.run(run_response())

