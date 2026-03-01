import os
import sys
import glob

def convert_csv_to_utf8():
    # 👉 [핵심 수정 부분] 환경에 따라 '현재 위치'를 다르게 잡도록 똑똑하게 수정합니다.
    if getattr(sys, 'frozen', False):
        # 1. PyInstaller로 만든 exe 파일로 실행된 경우
        script_dir = os.path.dirname(sys.executable)
    else:
        # 2. 일반 파이썬 스크립트(.py)로 실행된 경우
        script_dir = os.path.dirname(os.path.abspath(__file__))
    
    # 찾아낸 진짜 위치로 작업 폴더 변경
    os.chdir(script_dir)
    
    # 이하 코드는 기존과 동일합니다.
    csv_files = glob.glob('*.[cC][sS][vV]')
    
    if not csv_files:
        print("현재 폴더에 변환할 CSV 파일이 없습니다.")
        return

    print(f"총 {len(csv_files)}개의 CSV 파일을 발견했습니다. 변환을 시작합니다...\n")
    
    success_count = 0
    error_count = 0

    for file in csv_files:
        try:
            with open(file, 'r', encoding='cp949') as f:
                content = f.read()

            with open(file, 'w', encoding='utf-8') as f:
                f.write(content)
            
            print(f"✅ [성공] {file}")
            success_count += 1

        except UnicodeDecodeError:
            print(f"⚠️ [건너뜀] {file} - 이미 UTF-8로 되어있거나 다른 인코딩 방식입니다.")
            error_count += 1
        except Exception as e:
            print(f"❌ [에러] {file} - 문제 발생: {e}")
            error_count += 1

    print("-" * 40)
    print(f"🎉 작업 완료! (성공: {success_count}개 / 건너뜀 및 에러: {error_count}개)")

if __name__ == "__main__":
    convert_csv_to_utf8()
    input("\n엔터(Enter) 키를 누르면 창이 닫힙니다...")