import React, { useState } from 'react';
import { UnControlled, } from "react-codemirror2";
import { Hint, Toast } from "ui";
import { Copy } from "icons";
import CodeMirror from "./Exercise";

import classNames from "classnames";
import PropTypes from 'prop-types';

import styles from './Exercise.less';
import texts from "src/components/course/Course/Slide/Blocks/Exercise/Exercise.texts";


function StaticCode({ language, code, className, isHidden, }) {
	const lines = code.split('\n');
	const [collapseEditor, showAllCode] = useState(isHidden && lines.length > 20);

	const opts = {
		mode: CodeMirror.loadLanguageStyles(language),
		lineNumbers: true,
		lineWrapping: true,
		scrollbarStyle: 'null',
		theme: 'default',
		readOnly: true,
		matchBrackets: true,
	}

	const value = collapseEditor ? lines.splice(0, 5).join('\n') : code;

	return (
		<div className={ classNames(styles.wrapper, className) }>
			<UnControlled
				editorDidMount={ onEditorMount }
				className={ classNames(styles.editor, { [styles.lastLinesFading]: collapseEditor }) }
				options={ opts }
				value={ value }
			/>
			{ collapseEditor &&
			<React.Fragment>
				<div className={ styles.showAllCodeButton } onClick={ showAllCodeButtonClicked }>
					{ texts.controls.showAllCode.text }
				</div>
				<div className={ styles.copyCodeButton } onClick={ copyCodeButtonClicked }>
					<Hint text={ texts.controls.copyCode.text }>
						<Copy size={ 20 }/>
					</Hint>
				</div>
			</React.Fragment>
			}
		</div>
	);

	function onEditorMount(editor) {
		editor.setSize('auto', '100%')
	}

	function showAllCodeButtonClicked() {
		showAllCode(false);
	}

	function copyCodeButtonClicked() {
		navigator.clipboard.writeText(code);
		Toast.push(texts.controls.copyCode.onCopy);
	}
}

StaticCode.propTypes = {
	value: PropTypes.string,
	className: PropTypes.string,
	isHidden: PropTypes.bool,
}

export default StaticCode;