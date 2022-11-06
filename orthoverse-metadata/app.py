from flask import *

app = Flask(__name__)

@app.route('/<int:id>') 
def root(id):
    x=int(id/10000000)
    y=id%10000000
    retjson={"description": "OrthoverseZero Record","external_url": f'https://ortv.tech/{id}',"image": f'https://ortv.tech/ortv/20/{x}/{y}.png',"name": f'{id}'}

    return jsonify(retjson)

if __name__ == '__main__':
    app.run()

