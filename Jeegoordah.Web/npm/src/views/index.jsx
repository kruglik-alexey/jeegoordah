import React from 'react'
import Total from './total/total'
import P2PList from './p2pList'
import {withRouter} from 'react-router'

const P2PListWithRouter = withRouter(P2PList);

export default function () {
    return (
        <div className="container-fluid">
            <div className="row">
                <div className="col-xs-3">
                    <Total />
                </div>
                <div className="col-xs-1"></div>
                <div className="col-xs-8">
                    <P2PListWithRouter />
                </div>
            </div>
        </div>
    );
}