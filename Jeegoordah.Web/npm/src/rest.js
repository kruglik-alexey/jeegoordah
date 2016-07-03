import stubData from './stubData'

export function get(path) {
    return new Promise(resolve => {
        setTimeout(() => resolve(stubData[path]), 500);
    });
}