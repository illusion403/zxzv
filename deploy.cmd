@echo off
setlocal ENABLEDELAYEDEXPANSION

REM ================================
REM AWS KEYS (DEFINE ONCE)
REM ================================

REM --- Key A : PC -> S3 Bucket A
set KEYA_ACCESS=AKIA_S3A_XXXXXXXX
set KEYA_SECRET=S3A_SECRET_XXXXXXXX

REM --- Key B : S3 Bucket A -> Bucket B
set KEYB_ACCESS=AKIA_S3B_XXXXXXXX
set KEYB_SECRET=S3B_SECRET_XXXXXXXX

REM --- Key C : SSM -> EC2
set KEYC_ACCESS=AKIA_EC2_XXXXXXXX
set KEYC_SECRET=EC2_SECRET_XXXXXXXX

REM ================================
REM COMMON CONFIG
REM ================================
set REGION=ap-southeast-1

set LOCAL_FILE=C:\temp\app.zip
set S3_KEY=deploy/app.zip

set S3_BUCKET_A=my-bucket-a
set S3_BUCKET_B=my-bucket-b

set INSTANCE_ID=i-0123456789abcdef0

REM ================================
REM STEP 1: PC -> S3 Bucket A (Key A)
REM ================================
echo Uploading PC -> S3 Bucket A...

set AWS_ACCESS_KEY_ID=%KEYA_ACCESS%
set AWS_SECRET_ACCESS_KEY=%KEYA_SECRET%
set AWS_DEFAULT_REGION=%REGION%
set AWS_SESSION_TOKEN=

aws s3 cp "%LOCAL_FILE%" "s3://%S3_BUCKET_A%/%S3_KEY%"
IF ERRORLEVEL 1 (
    echo ❌ Upload failed
    exit /b 1
)

REM ================================
REM STEP 2: S3 Bucket A -> Bucket B (Key B)
REM ================================
echo Copying S3 Bucket A -> Bucket B...

set AWS_ACCESS_KEY_ID=%KEYB_ACCESS%
set AWS_SECRET_ACCESS_KEY=%KEYB_SECRET%
set AWS_DEFAULT_REGION=%REGION%
set AWS_SESSION_TOKEN=

aws s3 cp "s3://%S3_BUCKET_A%/%S3_KEY%" "s3://%S3_BUCKET_B%/%S3_KEY%"
IF ERRORLEVEL 1 (
    echo ❌ S3 to S3 copy failed
    exit /b 1
)

REM ================================
REM STEP 3: S3 Bucket B -> EC2 (Key C)
REM ================================
echo Sending SSM command to EC2...

set AWS_ACCESS_KEY_ID=%KEYC_ACCESS%
set AWS_SECRET_ACCESS_KEY=%KEYC_SECRET%
set AWS_DEFAULT_REGION=%REGION%
set AWS_SESSION_TOKEN=

aws ssm send-command ^
  --instance-ids "%INSTANCE_ID%" ^
  --document-name "AWS-RunPowerShellScript" ^
  --comment "Copy from S3 Bucket B to C:\\temp" ^
  --parameters commands=[
    "if (!(Test-Path 'C:\\temp')) { New-Item -ItemType Directory -Path 'C:\\temp' }",
    "aws s3 cp s3://%S3_BUCKET_B%/%S3_KEY% C:\\temp\\app.zip"
  ]

IF ERRORLEVEL 1 (
    echo ❌ SSM command failed
    exit /b 1
)

echo ✅ Done
pause
