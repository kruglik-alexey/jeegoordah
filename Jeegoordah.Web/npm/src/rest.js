import stubData from './stubData'

const delays = {
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