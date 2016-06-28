import stubData from './stubData'

export function get(path) {
    return new Promise(resolve => {
        resolve(stubData[path]);
    });
}