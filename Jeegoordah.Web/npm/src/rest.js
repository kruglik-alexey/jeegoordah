import stubData from './stubData'

const delays = {
    createTransaction: 10000
};

export function get(path) {
    if (path.match(/rates\//)) {
        path = 'rates';
    }
    return new Promise(resolve => {
        const delay = delays[path] || 100;
        setTimeout(() => resolve(stubData[path]), delay);
    });
}

export function post(path, data) {
    return new Promise(resolve => {
        const delay = delays[path] || 100;
        setTimeout(() => {
            if (path === 'createTransaction') {
                data.id = stubData.p2p[stubData.p2p.length - 1].id;
                stubData.p2p.push(data);
            }
            resolve();
        }, delay);
    });
}