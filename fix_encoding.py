#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
파일 인코딩 변환 스크립트
ISO-8859-1 또는 기타 인코딩을 UTF-8 BOM으로 변환합니다.

사용법:
    python fix_encoding.py                    # 전체 스캔 및 미리보기
    python fix_encoding.py --fix              # 실제 변환 실행
    python fix_encoding.py --path Assets/02_Scripts/Player  # 특정 폴더만
"""

import os
import sys
import argparse
from pathlib import Path
import chardet

# 변환 대상 확장자
TARGET_EXTENSIONS = {'.cs'}

# UTF-8 BOM
UTF8_BOM = b'\xef\xbb\xbf'


def detect_encoding(file_path):
    """파일의 인코딩을 감지합니다."""
    try:
        with open(file_path, 'rb') as f:
            raw_data = f.read()
            
        # BOM 체크
        if raw_data.startswith(UTF8_BOM):
            return 'utf-8-sig', True  # Already UTF-8 with BOM
        
        # chardet으로 인코딩 감지
        result = chardet.detect(raw_data)
        encoding = result['encoding']
        confidence = result['confidence']
        
        return encoding, False
    except Exception as e:
        print(f"Error detecting encoding for {file_path}: {e}")
        return None, False


def convert_to_utf8_bom(file_path, dry_run=True):
    """파일을 UTF-8 BOM으로 변환합니다."""
    try:
        # 현재 인코딩 감지
        current_encoding, already_utf8_bom = detect_encoding(file_path)
        
        if already_utf8_bom:
            return 'already_utf8_bom', current_encoding
        
        if current_encoding is None:
            return 'error', 'cannot_detect'
        
        # 파일 읽기
        with open(file_path, 'r', encoding=current_encoding, errors='ignore') as f:
            content = f.read()
        
        # Dry run이 아니면 실제로 변환
        if not dry_run:
            # UTF-8 BOM으로 저장
            with open(file_path, 'w', encoding='utf-8-sig') as f:
                f.write(content)
            return 'converted', current_encoding
        else:
            return 'needs_conversion', current_encoding
            
    except Exception as e:
        print(f"Error converting {file_path}: {e}")
        return 'error', str(e)


def scan_directory(root_path, target_extensions=TARGET_EXTENSIONS):
    """디렉토리를 스캔하여 대상 파일 목록을 반환합니다."""
    files = []
    root = Path(root_path)
    
    for ext in target_extensions:
        files.extend(root.rglob(f'*{ext}'))
    
    return files


def main():
    parser = argparse.ArgumentParser(
        description='C# 파일의 인코딩을 UTF-8 BOM으로 변환합니다.'
    )
    parser.add_argument(
        '--path',
        default='Assets/02_Scripts',
        help='스캔할 경로 (기본값: Assets/02_Scripts)'
    )
    parser.add_argument(
        '--fix',
        action='store_true',
        help='실제로 파일을 변환합니다 (없으면 미리보기만)'
    )
    parser.add_argument(
        '--verbose',
        action='store_true',
        help='자세한 출력'
    )
    
    args = parser.parse_args()
    
    # 경로 확인
    if not os.path.exists(args.path):
        print(f"Error: 경로를 찾을 수 없습니다: {args.path}")
        return 1
    
    print(f"🔍 스캔 중: {args.path}")
    files = scan_directory(args.path)
    print(f"📁 발견된 파일: {len(files)}개")
    print()
    
    # 통계
    stats = {
        'already_utf8_bom': 0,
        'needs_conversion': 0,
        'converted': 0,
        'error': 0
    }
    
    encoding_stats = {}
    
    # 각 파일 처리
    for file_path in files:
        result, encoding = convert_to_utf8_bom(file_path, dry_run=not args.fix)
        stats[result] += 1
        
        if encoding not in encoding_stats:
            encoding_stats[encoding] = 0
        encoding_stats[encoding] += 1
        
        # Verbose 모드 또는 변환이 필요한 경우 출력
        if args.verbose or result == 'needs_conversion' or result == 'converted':
            icon = {
                'already_utf8_bom': '✅',
                'needs_conversion': '⚠️ ',
                'converted': '✅',
                'error': '❌'
            }.get(result, '❓')
            
            relative_path = os.path.relpath(file_path, args.path)
            print(f"{icon} {relative_path}")
            if args.verbose:
                print(f"   현재 인코딩: {encoding}")
                print(f"   상태: {result}")
            print()
    
    # 결과 출력
    print("=" * 60)
    print("📊 변환 결과:")
    print("=" * 60)
    print(f"✅ 이미 UTF-8 BOM: {stats['already_utf8_bom']}개")
    print(f"⚠️  변환 필요: {stats['needs_conversion']}개")
    print(f"✅ 변환 완료: {stats['converted']}개")
    print(f"❌ 오류: {stats['error']}개")
    print()
    
    print("📈 인코딩 분포:")
    print("=" * 60)
    for encoding, count in sorted(encoding_stats.items(), key=lambda x: -x[1]):
        print(f"{encoding}: {count}개")
    print()
    
    # 변환이 필요한 경우 안내
    if not args.fix and stats['needs_conversion'] > 0:
        print("=" * 60)
        print("💡 실제로 변환하려면 --fix 옵션을 추가하세요:")
        print(f"   python fix_encoding.py --path {args.path} --fix")
        print("=" * 60)
        print()
        print("⚠️  주의: 변환 전에 반드시 Git으로 커밋하거나 백업하세요!")
    elif args.fix and stats['converted'] > 0:
        print("=" * 60)
        print("✅ 변환이 완료되었습니다!")
        print()
        print("다음 단계:")
        print("1. Unity 에디터에서 컴파일 에러가 없는지 확인")
        print("2. Git diff로 변경 사항 확인")
        print("3. 변경사항을 커밋:")
        print("   git add .")
        print('   git commit -m "Fix: Convert files to UTF-8 BOM encoding"')
        print("=" * 60)
    
    return 0


if __name__ == '__main__':
    sys.exit(main())
