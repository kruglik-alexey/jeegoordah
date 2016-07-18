import stubData from './stubData'

const delays = {
};

export function get(path) {
    return new Promise(resolve => {
        const delay = delays[path] || 100;
        setTimeout(() => resolve(stubData[path]), delay);
    });
}