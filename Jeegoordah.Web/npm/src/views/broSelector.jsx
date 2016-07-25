import React from 'react'
import _ from 'lodash'
import classNames from 'classnames'

export default class extends React.Component {
    render() {
        const lineSize = 70;
        let selected = this.props.multiSelect ? this.props.selected : [this.props.selected];
        selected = selected || [];
        const bros = this.props.bros.filter(b => !b.isHidden || selected.includes(b.id));
        const lineCount = bros.length / lineSize;
        const lines = [];

        for (var i = 0; i < lineCount; i++) {
            const bs = _
                .slice(bros, i * lineSize, (i + 1) * lineSize)
                .map(b => {
                    const cs = classNames('btn', 'btn-default', {active: selected.includes(b.id)});
                    return (
                        <label className={cs} key={b.id} onClick={() => this.broClick(b.id)}>
                            {b.name}
                        </label>
                    );
                });

            const style = {};
            if (i < lineCount - 1) {
                style.paddingBottom = '5px';
            }

            const l = (
                <div className="row" key={i} style={style}>
                    <div className="col-sm-12">
                        <div className="btn-group">
                            {bs}
                        </div>
                    </div>
                </div>
            );
            lines.push(l);
        }

        return <div style={{textAlign: 'center'}}>{lines}</div>
    }

    broClick(bro) {
        if (this.props.multiSelect) {
            let s = _.slice(this.props.selected);
            if (s.includes(bro)) {
                s = _.without(s, bro);
            } else {
                s.push(bro);
            }
            this.props.onSelect(s);
        } else {
            this.props.onSelect(bro);
        }
    }
}